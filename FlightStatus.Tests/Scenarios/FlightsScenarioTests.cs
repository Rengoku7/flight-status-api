using System.Net;
using System.Net.Http.Json;
using FlightStatus.Api.Models;
using FlightStatus.Application.UseCases.Flights.Queries.GetFlights;
using Xunit;

namespace FlightStatus.Tests.Scenarios;

public class FlightsScenarioTests : IClassFixture<FlightStatusWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly FlightStatusWebAppFactory _factory;

    public FlightsScenarioTests(FlightStatusWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = "moderator", password = "moderator" });
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResult<LoginPayload>>();
        var token = loginResult?.Data?.Token ?? throw new InvalidOperationException("No token");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task GetFlights_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/flights");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetFlights_WithAuth_Returns200AndList()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/flights");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<List<FlightDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetFlights_WithOriginFilter_ReturnsFiltered()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/flights?origin=ALA");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<List<FlightDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task AddFlight_AsModerator_Returns200AndId()
    {
        var client = await CreateAuthenticatedClientAsync();
        var body = new
        {
            origin = "ALA",
            destination = "NQZ",
            departure = DateTimeOffset.UtcNow.AddHours(1),
            arrival = DateTimeOffset.UtcNow.AddHours(3),
            status = "InTime"
        };
        var response = await client.PostAsJsonAsync("/api/flights", body);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<FlightIdPayload>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.Data?.Id > 0);
    }

    [Fact]
    public async Task UpdateFlightStatus_ExistingFlight_Returns200()
    {
        var client = await CreateAuthenticatedClientAsync();
        var addResponse = await client.PostAsJsonAsync("/api/flights", new
        {
            origin = "TSE",
            destination = "ALA",
            departure = DateTimeOffset.UtcNow.AddHours(2),
            arrival = DateTimeOffset.UtcNow.AddHours(4),
            status = "InTime"
        });
        addResponse.EnsureSuccessStatusCode();
        var addResult = await addResponse.Content.ReadFromJsonAsync<ApiResult<FlightIdPayload>>();
        var flightId = addResult?.Data?.Id ?? 0;
        Assert.True(flightId > 0);

        var patchResponse = await client.PatchAsJsonAsync($"/api/flights/{flightId}/status", new { status = "Delayed" });
        patchResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateFlightStatus_NonExistentFlight_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PatchAsJsonAsync("/api/flights/999999/status", new { status = "Cancelled" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private class LoginPayload
    {
        public string Token { get; set; } = "";
        public DateTimeOffset ExpiresAt { get; set; }
    }

    private class FlightIdPayload
    {
        public int Id { get; set; }
    }
}
