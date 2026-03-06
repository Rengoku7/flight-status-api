using System.ComponentModel.DataAnnotations;

namespace FlightStatus.Domain.Entities;

/// <summary>
/// Пользователь системы
/// </summary>
public class User
{
    /// <summary>Первичный ключ.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Имя пользователя (уникальное)</summary>
    [MaxLength(256)]
    public string Username { get; set; } = string.Empty;

    /// <summary>Хеш пароля.</summary>
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Внешний ключ на роль.</summary>
    public int RoleId { get; set; }

    /// <summary>Нав. свойство — роль пользователя.</summary>
    public Role? Role { get; set; }
}
