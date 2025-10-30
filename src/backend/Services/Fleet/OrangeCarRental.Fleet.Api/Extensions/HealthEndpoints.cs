namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Extensions;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new
        {
            service = "Fleet API",
            status = "Healthy",
            timestamp = DateTime.UtcNow
        }))
        .WithTags("Health")
        .WithName("HealthCheck");

        return app;
    }
}
