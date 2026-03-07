using FlightStatus.Application.Abstractions.Contracts;

namespace FlightStatus.Application.UseCases.Auth.Commands.Login;

public class LoginCommand : ICommand<LoginResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}
