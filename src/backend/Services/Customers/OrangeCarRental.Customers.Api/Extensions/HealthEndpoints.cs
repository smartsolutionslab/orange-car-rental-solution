using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Extensions;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        var health = app.MapGroup("/health")
            .WithTags("Health")
            .WithOpenApi();

        // GET /health - Basic health check
        health.MapGet("/", () => Results.Ok(new
        {
            status = "healthy",
            service = "Customers API",
            timestamp = DateTime.UtcNow
        }))
        .WithName("HealthCheck")
        .WithSummary("Health check")
        .WithDescription("Returns the health status of the Customers API.");

        // GET /health/ready - Readiness check (includes database connectivity)
        health.MapGet("/ready", async (CustomersDbContext dbContext, CancellationToken cancellationToken) =>
        {
            try
            {
                await dbContext.Database.CanConnectAsync(cancellationToken);
                return Results.Ok(new
                {
                    status = "ready",
                    service = "Customers API",
                    database = "connected",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new
                {
                    status = "not ready",
                    service = "Customers API",
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
