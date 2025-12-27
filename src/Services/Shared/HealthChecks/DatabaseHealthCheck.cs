using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace OrangeCarRental.Shared.HealthChecks;

/// <summary>
/// Health check for database connectivity and responsiveness
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly DbContext _context;
    private readonly TimeSpan _timeout;

    public DatabaseHealthCheck(DbContext context, TimeSpan? timeout = null)
    {
        _context = context;
        _timeout = timeout ?? TimeSpan.FromSeconds(5);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout);

            // Test database connection
            var canConnect = await _context.Database.CanConnectAsync(cts.Token);

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy(
                    "Database connection failed",
                    exception: null,
                    data: new Dictionary<string, object>
                    {
                        ["database"] = _context.Database.GetConnectionString() ?? "unknown"
                    });
            }

            // Test database query performance
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cts.Token);
            stopwatch.Stop();

            var responseTime = stopwatch.ElapsedMilliseconds;
            var status = responseTime switch
            {
                < 100 => HealthStatus.Healthy,
                < 500 => HealthStatus.Degraded,
                _ => HealthStatus.Unhealthy
            };

            return new HealthCheckResult(
                status,
                description: $"Database responded in {responseTime}ms",
                data: new Dictionary<string, object>
                {
                    ["responseTimeMs"] = responseTime,
                    ["database"] = _context.Database.GetConnectionString() ?? "unknown",
                    ["provider"] = _context.Database.ProviderName ?? "unknown"
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database health check failed",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["database"] = _context.Database.GetConnectionString() ?? "unknown"
                });
        }
    }
}
