using FlightStatus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightStatus.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        await SeedRolesAsync(db, ct);
        await SeedModeratorAsync(db, ct);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Roles.AnyAsync(ct))
            return;

        db.Roles.AddRange(
            new Role { Code = "Moderator" },
            new Role { Code = "Reader" });
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedModeratorAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Username == "moderator", ct))
            return;

        var moderatorRole = await db.Roles.FirstAsync(r => r.Code == "Moderator", ct);
        db.Users.Add(new User
        {
            Username = "moderator",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("moderator"),
            RoleId = moderatorRole.Id
        });
        await db.SaveChangesAsync(ct);
    }
}
