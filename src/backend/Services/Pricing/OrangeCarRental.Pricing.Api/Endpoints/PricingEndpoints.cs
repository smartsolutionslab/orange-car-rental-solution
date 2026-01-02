using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Pricing.Api.Requests;
using SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Api.Endpoints;

public static class PricingEndpoints
{
    public static IEndpointRouteBuilder MapPricingEndpoints(this IEndpointRouteBuilder app)
    {
        var pricing = app.MapGroup("/api/pricing")
            .WithTags("Pricing");

        // POST /api/pricing/calculate - Calculate rental price
        pricing.MapPost("/calculate", async (
                CalculatePriceRequest request,
                IQueryHandler<CalculatePriceQuery, PriceCalculationResult> handler,
                CancellationToken cancellationToken) =>
            {
                // Map request DTO to query with value objects
                var query = new CalculatePriceQuery(
                    CategoryCode.From(request.CategoryCode),
                    request.PickupDate,
                    request.ReturnDate,
                    !string.IsNullOrWhiteSpace(request.LocationCode)
                        ? LocationCode.From(request.LocationCode)
                        : null);

                var result = await handler.HandleAsync(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("CalculatePrice")
            .WithSummary("Calculate rental price")
            .WithDescription(
                "Calculates the total rental price for a vehicle category and period with German VAT (19%). Supports location-specific pricing if configured.")
            .Produces<PriceCalculationResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        return app;
    }
}
