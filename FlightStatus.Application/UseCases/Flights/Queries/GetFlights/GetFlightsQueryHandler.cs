using System.Text.Json;
using FlightStatus.Application.Abstractions;
using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

/// <summary>Обработчик запроса списка рейсов</summary>
public class GetFlightsQueryHandler : IQueryHandler<GetFlightsQuery, PagedResult<FlightDto>>
{
    private readonly IFlightsRepository _repo;
    private readonly IDistributedCache _cache;

    public GetFlightsQueryHandler(IFlightsRepository repo, IDistributedCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<PagedResult<FlightDto>>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var version = await _cache.GetStringAsync(FlightsCacheKeys.VersionKey, cancellationToken) ?? "1";
        var cacheKey = FlightsCacheKeys.GetFlightsKey(version, request.Origin, request.Destination, request.Page, request.PageSize);

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            var cachedResult = JsonSerializer.Deserialize<PagedResult<FlightDto>>(cached);
            if (cachedResult is not null)
                return Result<PagedResult<FlightDto>>.Success(cachedResult);
        }

        var (items, totalCount) = await _repo.GetFlightsPagedAsync(
            request.Origin, request.Destination, request.Page, request.PageSize, cancellationToken);
        var dtos = items.Select(f => new FlightDto
        {
            Id = f.Id,
            Origin = f.Origin,
            Destination = f.Destination,
            Departure = f.Departure,
            Arrival = f.Arrival,
            Status = f.Status.ToString()
        }).ToList();

        var result = new PagedResult<FlightDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var serialized = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(
            cacheKey,
            serialized,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            },
            cancellationToken);

        return Result<PagedResult<FlightDto>>.Success(result);
    }
}
