using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Application.UseCases.Flights.Commands.UpdateFlightStatus;

/// <summary>Обновляет только статус рейса; при отсутствии рейса — NotFound.</summary>
public class UpdateFlightStatusCommandHandler : ICommandHandler<UpdateFlightStatusCommand>
{
    private readonly IFlightsRepository _repo;

    public UpdateFlightStatusCommandHandler(IFlightsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<Unit>> Handle(UpdateFlightStatusCommand request, CancellationToken cancellationToken)
    {
        var flight = await _repo.GetByIdAsync(request.FlightId, cancellationToken);
        if (flight == null)
            return Result<Unit>.Failure(new Error("NotFound", "Рейс не найден"));

        flight.Status = request.Status;
        await _repo.UpdateAsync(flight, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
