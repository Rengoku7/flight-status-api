using FlightStatus.Application.Abstractions.Contracts;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

/// <summary>Запрос списка рейсов с фильтром по маршруту.</summary>
public class GetFlightsQuery : IQuery<List<FlightDto>>
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
}

/// <summary>DTO списка рейсов для ответа API.</summary>
public class FlightDto
{
    public int Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    /// <summary>Статус рейса строкой (InTime, Delayed, Cancelled).</summary>
    public string Status { get; set; } = string.Empty;
}
