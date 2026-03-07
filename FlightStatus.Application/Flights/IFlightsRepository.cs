using FlightStatus.Domain.Entities;

namespace FlightStatus.Application.Flights;

public interface IFlightsRepository
{
    Task<IReadOnlyList<Flight>> GetFlightsAsync(string? origin, string? destination, CancellationToken ct = default);
    Task<Flight?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> AddAsync(Flight flight, CancellationToken ct = default);
    Task UpdateAsync(Flight flight, CancellationToken ct = default);
}
