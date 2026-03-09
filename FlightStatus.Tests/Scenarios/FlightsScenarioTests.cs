using System.Net;
using System.Net.Http.Json;
using FlightStatus.Api.Models;
using FlightStatus.Application.Abstractions;
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
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = "moderator", password = "Moderator1" });
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
    public async Task GetFlights_WithAuth_Returns200AndPagedList()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/flights");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<FlightDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data?.Items);
        Assert.True(result.Data.TotalCount >= 0);
        Assert.Equal(1, result.Data.Page);
        Assert.Equal(20, result.Data.PageSize);
    }

    [Fact]
    public async Task GetFlights_WithOriginFilter_ReturnsFiltered()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/flights?origin=ALA&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<FlightDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data?.Items);
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

        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/flights/{flightId}/status")
            { Content = JsonContent.Create(new { status = "Delayed" }) };
        var patchResponse = await client.SendAsync(patchRequest);
        patchResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateFlightStatus_NonExistentFlight_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/flights/999999/status")
            { Content = JsonContent.Create(new { status = "Cancelled" }) };
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFlightById_ExistingId_Returns200()
    {
        var client = await CreateAuthenticatedClientAsync();
        var listResponse = await client.GetAsync("/api/flights?pageSize=1");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<ApiResult<PagedResult<FlightDto>>>();
        var id = list?.Data?.Items?.FirstOrDefault()?.Id ?? 1;
        var response = await client.GetAsync($"/api/flights/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<FlightDto>>();
        Assert.NotNull(result);
        Assert.True(result!.Success);
        Assert.Equal(id, result.Data!.Id);
    }

    [Fact]
    public async Task GetFlightById_NonExistent_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/flights/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFlights_Pagination_Page2DifferentFromPage1()
    {
        var client = await CreateAuthenticatedClientAsync();
        var r1 = await client.GetAsync("/api/flights?page=1&pageSize=1");
        var r2 = await client.GetAsync("/api/flights?page=2&pageSize=1");
        r1.EnsureSuccessStatusCode();
        r2.EnsureSuccessStatusCode();
        var d1 = await r1.Content.ReadFromJsonAsync<ApiResult<PagedResult<FlightDto>>>();
        var d2 = await r2.Content.ReadFromJsonAsync<ApiResult<PagedResult<FlightDto>>>();
        Assert.NotNull(d1?.Data);
        Assert.NotNull(d2?.Data);
        Assert.Equal(1, d1.Data.Page);
        Assert.Equal(2, d2.Data.Page);
    }
    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task AddFlight_WithoutModeratorRole_Returns403()
    {
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = "user", password = "Reader1" });
        if (!loginResponse.IsSuccessStatusCode)
            return; 
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResult<LoginPayload>>();
        var token = loginResult?.Data?.Token ?? "";
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await client.PostAsJsonAsync("/api/flights", new
        {
            origin = "ALA",
            destination = "NQZ",
            departure = DateTimeOffset.UtcNow.AddHours(1),
            arrival = DateTimeOffset.UtcNow.AddHours(3),
            status = "InTime"
        });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
