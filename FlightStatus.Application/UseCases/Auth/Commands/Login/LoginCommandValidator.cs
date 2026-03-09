using FluentValidation;

namespace FlightStatus.Application.UseCases.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private const int MinPasswordLength = 8;
    private static readonly string PasswordRules = "минимум 8 символов, хотя бы одна буква и одна цифра";

    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(MinPasswordLength).WithMessage($"Пароль: {PasswordRules}")
            .Must(BeValidPassword).WithMessage($"Пароль: {PasswordRules}");
    }

    private static bool BeValidPassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            return false;
        return password.Any(char.IsLetter) && password.Any(char.IsDigit);
    }
}
