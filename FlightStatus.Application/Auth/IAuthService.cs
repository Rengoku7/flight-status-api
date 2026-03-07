namespace FlightStatus.Application.Auth;

public interface IAuthService
{
    Task<UserInfo?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
}
