using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Application.UseCases.Flights.Queries.GetFlights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlightById;

public class GetFlightByIdQueryHandler : IQueryHandler<GetFlightByIdQuery, FlightDto>
{
    private readonly IFlightsRepository _repo;

    public GetFlightByIdQueryHandler(IFlightsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<FlightDto>> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
    {
        var flight = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (flight == null)
            throw new ApplicationNotFoundException("Рейс не найден");
        return Result<FlightDto>.Success(new FlightDto
        {
            Id = flight.Id,
            Origin = flight.Origin,
            Destination = flight.Destination,
            Departure = flight.Departure,
            Arrival = flight.Arrival,
            Status = flight.Status.ToString()
        });
    }
}
