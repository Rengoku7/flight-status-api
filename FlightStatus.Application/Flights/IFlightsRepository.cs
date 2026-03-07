using FlightStatus.Domain.Entities;

namespace FlightStatus.Application.Flights;

/// <summary>Репозиторий рейсов</summary>
public interface IFlightsRepository
{
    /// <param name="origin">Фильтр по пункту вылета (опционально).</param>
    /// <param name="destination">Фильтр по пункту назначения (опционально).</param>
    Task<IReadOnlyList<Flight>> GetFlightsAsync(string? origin, string? destination, CancellationToken ct = default);
    Task<Flight?> GetByIdAsync(int id, CancellationToken ct = default);
    /// <returns>Id созданного рейса.</returns>
    Task<int> AddAsync(Flight flight, CancellationToken ct = default);
    Task UpdateAsync(Flight flight, CancellationToken ct = default);
}
