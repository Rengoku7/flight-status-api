using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlightById;

public class GetFlightByIdQuery : IQuery<FlightDto>
{
    public int Id { get; set; }
}
