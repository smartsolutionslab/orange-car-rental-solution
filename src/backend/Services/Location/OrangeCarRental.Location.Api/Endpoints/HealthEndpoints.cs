namespace SmartSolutionsLab.OrangeCarRental.Location.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "location" }))
            .WithName("HealthCheck")
            .WithTags("Health");

        return app;
    }
}
