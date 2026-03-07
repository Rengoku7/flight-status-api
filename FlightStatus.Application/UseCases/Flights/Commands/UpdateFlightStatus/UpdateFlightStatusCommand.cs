using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Domain.Enums;

namespace FlightStatus.Application.UseCases.Flights.Commands.UpdateFlightStatus;

public class UpdateFlightStatusCommand : ICommand
{
    public int FlightId { get; set; }
    public FlightStatusKind Status { get; set; }
}
