using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace FlightStatus.Infrastructure.Persistence;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditSaveChangesInterceptor> _logger;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuditSaveChangesInterceptor(
        ILogger<AuditSaveChangesInterceptor> logger,
        IHttpContextAccessor? httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is DbContext context)
            LogChanges(context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is DbContext context)
            LogChanges(context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void LogChanges(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
        if (entries.Count == 0) return;

        var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
        var sb = new StringBuilder();
        foreach (var entry in entries)
        {
            sb.Append($"[{entry.State}] {entry.Entity.GetType().Name}");
            if (entry.Metadata.FindPrimaryKey() is { } key)
            {
                var id = entry.Property(key.Properties.First().Name).CurrentValue;
                sb.Append($" Id={id}");
            }
            sb.Append("; ");
        }
        _logger.LogInformation(
            "БД: пользователь={User}, время={Time:yyyy-MM-dd HH:mm:ss}, изменения: {Changes}",
            userName, DateTime.UtcNow, sb.ToString().TrimEnd());
    }
}
