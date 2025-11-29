namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Extensions;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "payments" }))
            .WithName("HealthCheck")
            .WithTags("Health");

        return app;
    }
}
