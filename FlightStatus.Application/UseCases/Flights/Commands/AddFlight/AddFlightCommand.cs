using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Domain.Enums;

namespace FlightStatus.Application.UseCases.Flights.Commands.AddFlight;

/// <summary>Команда создания рейса (тело POST api/flights).</summary>
public class AddFlightCommand : ICommand<FlightIdResult>
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public FlightStatusKind Status { get; set; }
}

/// <summary>Ответ после создания рейса.</summary>
public class FlightIdResult
{
    public int Id { get; set; }
}
