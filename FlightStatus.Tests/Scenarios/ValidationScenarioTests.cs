using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FlightStatus.Api.Models;
using Xunit;

namespace FlightStatus.Tests.Scenarios;

public class ValidationScenarioTests : IClassFixture<FlightStatusWebAppFactory>
{
    private readonly FlightStatusWebAppFactory _factory;

    public ValidationScenarioTests(FlightStatusWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_EmptyUsername_Returns400_WithErrors()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "", password = "Password1" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<object>>();
        Assert.NotNull(result);
        Assert.False(result!.Success);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task AddFlight_InvalidBody_Returns400_WithErrors()
    {
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = "moderator", password = "Moderator1" });
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResult<LoginPayload>>();
        var token = loginResult?.Data?.Token ?? "";
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new
        {
            origin = "",
            destination = "NQZ",
            departure = DateTimeOffset.UtcNow.AddHours(3),
            arrival = DateTimeOffset.UtcNow.AddHours(1),
            status = "InvalidStatus"
        };
        var response = await client.PostAsJsonAsync("/api/flights", body);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<object>>();
        Assert.NotNull(result);
        Assert.False(result!.Success);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
    }

    private class LoginPayload
    {
        public string Token { get; set; } = "";
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
