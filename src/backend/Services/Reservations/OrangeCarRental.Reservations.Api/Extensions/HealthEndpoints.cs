namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Extensions;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new
        {
            Status = "Healthy",
            Service = "Reservations"
        }))
        .WithName("HealthCheck")
        .WithTags("Health");

        return app;
    }
}
