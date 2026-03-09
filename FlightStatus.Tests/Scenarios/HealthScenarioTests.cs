using System.Net;
using Xunit;

namespace FlightStatus.Tests.Scenarios;

public class HealthScenarioTests : IClassFixture<FlightStatusWebAppFactory>
{
    private readonly HttpClient _client;

    public HealthScenarioTests(FlightStatusWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Returns200()
    {
        var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
