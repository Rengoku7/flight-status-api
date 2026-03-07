using FlightStatus.Application.Auth;
using FlightStatus.Application.Flights;
using FlightStatus.Infrastructure.Persistence;
using FlightStatus.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightStatus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFlightsRepository, FlightsRepository>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenService>();

        return services;
    }
}
