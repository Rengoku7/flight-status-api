namespace FlightStatus.Domain.Enums;

/// <summary>
/// Статус рейса.
/// </summary>
public enum FlightStatusKind
{
    /// <summary>Рейс по расписанию.</summary>
    InTime,

    /// <summary>Рейс задержан.</summary>
    Delayed,

    /// <summary>Рейс отменён.</summary>
    Cancelled
}
