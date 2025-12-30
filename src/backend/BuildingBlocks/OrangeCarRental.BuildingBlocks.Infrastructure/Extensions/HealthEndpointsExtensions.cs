using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
/// Extension methods for mapping reusable health check endpoints.
/// </summary>
public static class HealthEndpointsExtensions
{
    /// <summary>
    /// Maps health check endpoints for an API with database connectivity checks.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type for database connectivity checks</typeparam>
    /// <param name="app">The endpoint route builder</param>
    /// <param name="serviceName">The name of the service (e.g., "Customers API")</param>
    /// <returns>The endpoint route builder for chaining</returns>
    public static IEndpointRouteBuilder MapHealthEndpoints<TDbContext>(
        this IEndpointRouteBuilder app,
        string serviceName) where TDbContext : DbContext
    {
        var health = app.MapGroup("/health")
            .WithTags("Health");

        // GET /health - Basic health check
        health.MapGet("/",
                () => Results.Ok(new
                {
                    status = "healthy",
                    service = serviceName,
                    timestamp = DateTime.UtcNow
                }))
            .WithName("HealthCheck")
            .WithSummary("Health check")
            .WithDescription($"Returns the health status of the {serviceName}.");

        // GET /health/ready - Readiness check (includes database connectivity)
        health.MapGet("/ready", async (TDbContext dbContext, CancellationToken cancellationToken) =>
            {
                try
                {
                    await dbContext.Database.CanConnectAsync(cancellationToken);
                    return Results.Ok(new
                    {
                        status = "ready",
                        service = serviceName,
                        database = "connected",
                        timestamp = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    return Results.Json(
                        new
                        {
                            status = "not ready",
                            service = serviceName,
                            database = "disconnected",
                            error = ex.Message,
                            timestamp = DateTime.UtcNow
                        }, statusCode: StatusCodes.Status503ServiceUnavailable);
                }
            })
            .WithName("ReadinessCheck")
            .WithSummary("Readiness check")
            .WithDescription("Returns the readiness status including database connectivity.");

        return app;
    }
}
