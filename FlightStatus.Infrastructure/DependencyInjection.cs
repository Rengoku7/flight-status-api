using FlightStatus.Application.Auth;
using FlightStatus.Infrastructure.Persistence;
using FlightStatus.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightStatus.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрация сервисов слоя инфраструктуры
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenService>();

        return services;
    }
}
