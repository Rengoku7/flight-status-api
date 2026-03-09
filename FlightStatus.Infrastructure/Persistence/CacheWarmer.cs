using System.Text.Json;
using FlightStatus.Application.Abstractions;
using FlightStatus.Application.Flights;
using FlightStatus.Application.UseCases.Flights.Queries.GetFlights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FlightStatus.Infrastructure.Persistence;

/// <summary>Прогрев кэша рейсов после сида</summary>
public static class CacheWarmer
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public static async Task WarmFlightsCacheAsync(ApplicationDbContext db, IDistributedCache cache, CancellationToken ct = default)
    {
        const string version = "1";
        await cache.SetStringAsync(FlightsCacheKeys.VersionKey, version, new DistributedCacheEntryOptions(), ct);

        var flights = await db.Flights.OrderBy(f => f.Arrival).ToListAsync(ct);
        if (flights.Count == 0)
            return;

        var dtos = flights.Select(f => new FlightDto
        {
            Id = f.Id,
            Origin = f.Origin,
            Destination = f.Destination,
            Departure = f.Departure,
            Arrival = f.Arrival,
            Status = f.Status.ToString()
        }).ToList();

        foreach (var dto in dtos)
        {
            var key = FlightsCacheKeys.GetFlightKey(version, dto.Id);
            var json = JsonSerializer.Serialize(dto, JsonOptions);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            }, ct);
        }

        var firstPage = new PagedResult<FlightDto>
        {
            Items = dtos.Take(20).ToList(),
            TotalCount = dtos.Count,
            Page = 1,
            PageSize = 20
        };
        var listKey = FlightsCacheKeys.GetFlightsKey(version, null, null, 1, 20);
        var listJson = JsonSerializer.Serialize(firstPage, JsonOptions);
        await cache.SetStringAsync(listKey, listJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
        }, ct);
    }
}
