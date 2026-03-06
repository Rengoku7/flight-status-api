using System.ComponentModel.DataAnnotations;
using FlightStatus.Domain.Enums;

namespace FlightStatus.Domain.Entities;

/// <summary>
/// Рейс и данные о рейсе
/// </summary>
public class Flight
{
    /// <summary>Первичный ключ.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Пункт вылета (макс. 256 символов).</summary>
    [MaxLength(256)]
    public string Origin { get; set; } = string.Empty;

    /// <summary>Пункт назначения (макс. 256 символов).</summary>
    [MaxLength(256)]
    public string Destination { get; set; } = string.Empty;

    /// <summary>Время вылета (часовой пояс — относительно пункта вылета).</summary>
    public DateTimeOffset Departure { get; set; }

    /// <summary>Время прилёта (часовой пояс — относительно пункта назначения).</summary>
    public DateTimeOffset Arrival { get; set; }

    /// <summary>Текущий статус рейса.</summary>
    public FlightStatusKind Status { get; set; }
}
