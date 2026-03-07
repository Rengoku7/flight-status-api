using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Domain.Enums;

namespace FlightStatus.Application.UseCases.Flights.Commands.AddFlight;

public class AddFlightCommand : ICommand<FlightIdResult>
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public FlightStatusKind Status { get; set; }
}

public class FlightIdResult
{
    public int Id { get; set; }
}
