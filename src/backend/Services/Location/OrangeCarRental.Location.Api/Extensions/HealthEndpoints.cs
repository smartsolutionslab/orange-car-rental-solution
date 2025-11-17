namespace SmartSolutionsLab.OrangeCarRental.Location.Api.Extensions;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health",
                () => Results.Ok(new { service = "Location API", status = "Healthy", timestamp = DateTime.UtcNow }))
            .WithTags("Health")
            .WithName("HealthCheck");

        return app;
    }
}
