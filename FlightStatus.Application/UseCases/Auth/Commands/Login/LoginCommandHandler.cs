using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Auth;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly Microsoft.Extensions.Options.IOptions<JwtConfiguration> _jwtConfig;

    public LoginCommandHandler(
        IAuthService authService,
        IJwtTokenGenerator tokenGenerator,
        Microsoft.Extensions.Options.IOptions<JwtConfiguration> jwtConfig)
    {
        _authService = authService;
        _tokenGenerator = tokenGenerator;
        _jwtConfig = jwtConfig;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.ValidateCredentialsAsync(request.Username, request.Password, cancellationToken);
        if (user == null)
            return Result<LoginResponse>.Failure(Error.Unauthorized("Неверное имя пользователя или пароль"));

        var token = _tokenGenerator.CreateToken(user);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtConfig.Value.ExpirationMinutes);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt
        });
    }
}
