using FlightStatus.Domain.Entities;
using FlightStatus.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FlightStatus.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        await SeedRolesAsync(db, ct);
        await SeedUsersAsync(db, ct);
        await SeedFlightsAsync(db, ct);
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

    private static async Task SeedUsersAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Username == "moderator", ct))
            return;

        var moderatorRole = await db.Roles.FirstAsync(r => r.Code == "Moderator", ct);
        var readerRole = await db.Roles.FirstAsync(r => r.Code == "Reader", ct);

        db.Users.Add(new User
        {
            Username = "moderator",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Moderator1"),
            RoleId = moderatorRole.Id
        });
        db.Users.Add(new User
        {
            Username = "user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Reader1"),
            RoleId = readerRole.Id
        });
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedFlightsAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Flights.AnyAsync(ct))
            return;

        var now = DateTimeOffset.UtcNow;
        db.Flights.AddRange(
            new Flight
            {
                Origin = "ALA",
                Destination = "NQZ",
                Departure = now.AddHours(2),
                Arrival = now.AddHours(4),
                Status = FlightStatusKind.InTime
            },
            new Flight
            {
                Origin = "NQZ",
                Destination = "ALA",
                Departure = now.AddHours(5),
                Arrival = now.AddHours(7),
                Status = FlightStatusKind.InTime
            },
            new Flight
            {
                Origin = "TSE",
                Destination = "ALA",
                Departure = now.AddHours(1),
                Arrival = now.AddHours(3),
                Status = FlightStatusKind.Delayed
            });
        await db.SaveChangesAsync(ct);
    }
}
