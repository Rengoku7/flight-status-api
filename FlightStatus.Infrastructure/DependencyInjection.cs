using FlightStatus.Application.Auth;
using FlightStatus.Application.Flights;
using FlightStatus.Infrastructure.Persistence;
using FlightStatus.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightStatus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditSaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("TestDb");
                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });
        }

        // Кэш: Redis если указан, иначе in-memory распределённый кэш.
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "flight-status-api:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFlightsRepository, FlightsRepository>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenService>();

        return services;
    }
}
