using System.Net;
using System.Net.Http.Json;
using FlightStatus.Api.Models;
using Xunit;

namespace FlightStatus.Tests.Scenarios;

public class AuthScenarioTests : IClassFixture<FlightStatusWebAppFactory>
{
    private readonly HttpClient _client;

    public AuthScenarioTests(FlightStatusWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200AndToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "moderator",
            password = "moderator"
        });

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Login failed: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        var result = System.Text.Json.JsonSerializer.Deserialize<ApiResult<LoginData>>(body,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.True(result.Success, $"Success was false. Body: {body}");
        Assert.NotNull(result.Data?.Token);
        Assert.True(result.Data.ExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "moderator",
            password = "wrong"
        });

        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"Expected 401, got {(int)response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task Login_UnknownUser_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "nobody",
            password = "nobody"
        });

        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"Expected 401, got {(int)response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");
    }

    private class LoginData
    {
        public string Token { get; set; } = "";
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
