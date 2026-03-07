using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Entities;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Application.UseCases.Flights.Commands.AddFlight;

public class AddFlightCommandHandler : ICommandHandler<AddFlightCommand, FlightIdResult>
{
    private readonly IFlightsRepository _repo;

    public AddFlightCommandHandler(IFlightsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<FlightIdResult>> Handle(AddFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = new Flight
        {
            Origin = request.Origin,
            Destination = request.Destination,
            Departure = request.Departure,
            Arrival = request.Arrival,
            Status = request.Status
        };
        var id = await _repo.AddAsync(flight, cancellationToken);
        return Result<FlightIdResult>.Success(new FlightIdResult { Id = id });
    }
}
