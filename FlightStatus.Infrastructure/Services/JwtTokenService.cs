using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlightStatus.Application.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlightStatus.Infrastructure.Services;

public class JwtTokenService : IJwtTokenGenerator
{
    private readonly JwtConfiguration _config;

    public JwtTokenService(IOptions<JwtConfiguration> config)
    {
        _config = config.Value;
    }

    public string CreateToken(UserInfo user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_config.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.RoleCode)
        };

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
