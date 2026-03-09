using System.Text.Json;
using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Application.UseCases.Flights.Queries.GetFlights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlightById;

public class GetFlightByIdQueryHandler : IQueryHandler<GetFlightByIdQuery, FlightDto>
{
    private readonly IFlightsRepository _repo;
    private readonly IDistributedCache _cache;

    public GetFlightByIdQueryHandler(IFlightsRepository repo, IDistributedCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<FlightDto>> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
    {
        var version = await _cache.GetStringAsync(FlightsCacheKeys.VersionKey, cancellationToken) ?? "1";
        var cacheKey = FlightsCacheKeys.GetFlightKey(version, request.Id);

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            var cachedDto = JsonSerializer.Deserialize<FlightDto>(cached);
            if (cachedDto is not null)
                return Result<FlightDto>.Success(cachedDto);
        }

        var flight = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (flight == null)
            throw new ApplicationNotFoundException("Рейс не найден");
        var dto = new FlightDto
        {
            Id = flight.Id,
            Origin = flight.Origin,
            Destination = flight.Destination,
            Departure = flight.Departure,
            Arrival = flight.Arrival,
            Status = flight.Status.ToString()
        };

        var serialized = JsonSerializer.Serialize(dto);
        await _cache.SetStringAsync(
            cacheKey,
            serialized,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            },
            cancellationToken);

        return Result<FlightDto>.Success(dto);
    }
}
