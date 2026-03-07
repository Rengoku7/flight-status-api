namespace FlightStatus.Application.Auth;

public interface IJwtTokenGenerator
{
    string CreateToken(UserInfo user);
}
