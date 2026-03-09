using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FlightStatus.Application.UseCases.Flights.Commands.UpdateFlightStatus;

/// <summary>Обновляет только статус рейса; при отсутствии рейса — NotFound.</summary>
public class UpdateFlightStatusCommandHandler : ICommandHandler<UpdateFlightStatusCommand>
{
    private readonly IFlightsRepository _repo;
    private readonly IDistributedCache _cache;

    public UpdateFlightStatusCommandHandler(IFlightsRepository repo, IDistributedCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<Unit>> Handle(UpdateFlightStatusCommand request, CancellationToken cancellationToken)
    {
        var flight = await _repo.GetByIdAsync(request.FlightId, cancellationToken);
        if (flight == null)
            throw new ApplicationNotFoundException("Рейс не найден");

        flight.Status = request.Status;
        await _repo.UpdateAsync(flight, cancellationToken);
        await BumpCacheVersionAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }

    private async Task BumpCacheVersionAsync(CancellationToken cancellationToken)
    {
        var current = await _cache.GetStringAsync(FlightsCacheKeys.VersionKey, cancellationToken);
        if (!int.TryParse(current, out var version) || version <= 0)
            version = 1;
        version++;
        await _cache.SetStringAsync(
            FlightsCacheKeys.VersionKey,
            version.ToString(),
            new DistributedCacheEntryOptions(),
            cancellationToken);
    }
}
