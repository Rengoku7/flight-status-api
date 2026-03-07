using FlightStatus.Application.Abstractions.Contracts;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

public class GetFlightsQuery : IQuery<List<FlightDto>>
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
}

public class FlightDto
{
    public int Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public string Status { get; set; } = string.Empty;
}
