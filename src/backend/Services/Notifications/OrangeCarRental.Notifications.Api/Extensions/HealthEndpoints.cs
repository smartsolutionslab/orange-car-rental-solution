namespace SmartSolutionsLab.OrangeCarRental.Notifications.Api.Extensions;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        // Health check endpoint
        app.MapGet("/health",
                () => Results.Ok(new { service = "Notifications API", status = "Healthy", timestamp = DateTime.UtcNow }))
            .WithTags("Health")
            .WithName("HealthCheck");

        return app;
    }
}
