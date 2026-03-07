using FlightStatus.Application.Auth;
using FlightStatus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlightStatus.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserInfo?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

        if (user?.Role == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return new UserInfo(user.Id, user.Username, user.Role.Code);
    }
}
