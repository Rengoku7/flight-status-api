namespace FlightStatus.Domain.Abstractions.Primitive;

public sealed record Error(string Code, string? Message = null)
{
    public static Error None { get; } = new(string.Empty);

    public static Error Unauthorized(string message = "Неверные учётные данные") => new("Unauthorized", message);
    public static Error Validation(string message = "Ошибка валидации") => new("Validation", message);
}
