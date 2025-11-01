using SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Api.Extensions;

public static class PricingEndpoints
{
    public static IEndpointRouteBuilder MapPricingEndpoints(this IEndpointRouteBuilder app)
    {
        var pricing = app.MapGroup("/api/pricing")
            .WithTags("Pricing")
            .WithOpenApi();

        // POST /api/pricing/calculate - Calculate rental price
        pricing.MapPost("/calculate", async (
            CalculatePriceQuery query,
            CalculatePriceQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await handler.HandleAsync(query, cancellationToken);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("CalculatePrice")
        .WithSummary("Calculate rental price")
        .WithDescription("Calculates the total rental price for a vehicle category and period with German VAT (19%). Supports location-specific pricing if configured.")
        .Produces<PriceCalculationResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}
