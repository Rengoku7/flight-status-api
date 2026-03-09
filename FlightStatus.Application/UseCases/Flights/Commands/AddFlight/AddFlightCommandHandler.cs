using System.Text.Json;
using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Entities;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FlightStatus.Application.UseCases.Flights.Commands.AddFlight;

/// <summary>Создаёт рейс в БД и возвращает его id.</summary>
public class AddFlightCommandHandler : ICommandHandler<AddFlightCommand, FlightIdResult>
{
    private readonly IFlightsRepository _repo;
    private readonly IDistributedCache _cache;

    public AddFlightCommandHandler(IFlightsRepository repo, IDistributedCache cache)
    {
        _repo = repo;
        _cache = cache;
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

        await BumpCacheVersionAsync(cancellationToken);

        return Result<FlightIdResult>.Success(new FlightIdResult { Id = id });
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
