namespace FlightStatus.Application.Flights;

public static class FlightsCacheKeys
{
    public const string VersionKey = "flights:version";

    public static string GetFlightsKey(string version, string? origin, string? destination, int page, int pageSize) =>
        $"flights:v{version}:{origin ?? "_"}:{destination ?? "_"}:{page}:{pageSize}";

    public static string GetFlightKey(string version, int id) => $"flight:v{version}:{id}";
}

