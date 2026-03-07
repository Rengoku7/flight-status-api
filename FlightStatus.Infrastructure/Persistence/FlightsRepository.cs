using FlightStatus.Application.Flights;
using FlightStatus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightStatus.Infrastructure.Persistence;

public class FlightsRepository : IFlightsRepository
{
    private readonly ApplicationDbContext _db;

    public FlightsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<Flight> Items, int TotalCount)> GetFlightsPagedAsync(string? origin, string? destination, int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var query = _db.Flights.AsNoTracking();
            var useNpgsql = _db.Database.ProviderName?.Contains("Npgsql") == true;
            if (!string.IsNullOrWhiteSpace(origin))
                query = useNpgsql ? query.Where(f => EF.Functions.ILike(f.Origin, origin)) : query.Where(f => f.Origin.ToLower() == origin.ToLower());
            if (!string.IsNullOrWhiteSpace(destination))
                query = useNpgsql ? query.Where(f => EF.Functions.ILike(f.Destination, destination)) : query.Where(f => f.Destination.ToLower() == destination.ToLower());
            var totalCount = await query.CountAsync(ct);
            var items = await query.OrderBy(f => f.Arrival)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            return (items, totalCount);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("Ошибка при получении списка рейсов", ex);
        }
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _db.Flights.FindAsync(new object[] { id }, ct);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("Ошибка при получении рейса по id", ex);
        }
    }

    public async Task<int> AddAsync(Flight flight, CancellationToken ct = default)
    {
        try
        {
            _db.Flights.Add(flight);
            await _db.SaveChangesAsync(ct);
            return flight.Id;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("Ошибка при добавлении рейса", ex);
        }
    }

    public async Task UpdateAsync(Flight flight, CancellationToken ct = default)
    {
        try
        {
            _db.Flights.Update(flight);
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("Ошибка при обновлении рейса", ex);
        }
    }
}
