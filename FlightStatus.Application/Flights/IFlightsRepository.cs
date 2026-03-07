using FlightStatus.Domain.Entities;

namespace FlightStatus.Application.Flights;

/// <summary>Репозиторий рейсов</summary>
public interface IFlightsRepository
{
    /// <param name="origin">Фильтр по пункту вылета (опционально).</param>
    /// <param name="destination">Фильтр по пункту назначения (опционально).</param>
    /// <param name="page">Номер страницы</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <returns>Элементы страницы и общее количество записей.</returns>
    Task<(IReadOnlyList<Flight> Items, int TotalCount)> GetFlightsPagedAsync(string? origin, string? destination, int page, int pageSize, CancellationToken ct = default);
    Task<Flight?> GetByIdAsync(int id, CancellationToken ct = default);
    /// <returns>Id созданного рейса.</returns>
    Task<int> AddAsync(Flight flight, CancellationToken ct = default);
    Task UpdateAsync(Flight flight, CancellationToken ct = default);
}
