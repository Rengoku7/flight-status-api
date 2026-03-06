using System.ComponentModel.DataAnnotations;

namespace FlightStatus.Domain.Entities;

/// <summary>
/// Роль пользователя в системе.
/// </summary>
public class Role
{
    /// <summary>Первичный ключ.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Код роли (уникальный).</summary>
    [MaxLength(256)]
    public string Code { get; set; } = string.Empty;
}
