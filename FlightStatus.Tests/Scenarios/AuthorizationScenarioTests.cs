using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FlightStatus.Api.Models;
using Xunit;

namespace FlightStatus.Tests.Scenarios;

public class AuthorizationScenarioTests : IClassFixture<FlightStatusWebAppFactory>
{
    private readonly FlightStatusWebAppFactory _factory;

    public AuthorizationScenarioTests(FlightStatusWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFlights_WithInvalidToken_Returns401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.jwt.token");
        var response = await client.GetAsync("/api/flights");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetFlights_WithValidToken_Returns200()
    {
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = "moderator", password = "Moderator1" });
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResult<LoginPayload>>();
        var token = loginResult?.Data?.Token ?? "";
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/api/flights");
        response.EnsureSuccessStatusCode();
    }

    private class LoginPayload
    {
        public string Token { get; set; } = "";
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
