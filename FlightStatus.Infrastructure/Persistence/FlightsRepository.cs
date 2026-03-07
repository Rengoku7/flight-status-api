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

    public async Task<IReadOnlyList<Flight>> GetFlightsAsync(string? origin, string? destination, CancellationToken ct = default)
    {
        var query = _db.Flights.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(f => f.Origin == origin);
        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(f => f.Destination == destination);
        return await query.OrderBy(f => f.Arrival).ToListAsync(ct);
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Flights.FindAsync(new object[] { id }, ct);
    }

    public async Task<int> AddAsync(Flight flight, CancellationToken ct = default)
    {
        _db.Flights.Add(flight);
        await _db.SaveChangesAsync(ct);
        return flight.Id;
    }

    public async Task UpdateAsync(Flight flight, CancellationToken ct = default)
    {
        _db.Flights.Update(flight);
        await _db.SaveChangesAsync(ct);
    }
}
