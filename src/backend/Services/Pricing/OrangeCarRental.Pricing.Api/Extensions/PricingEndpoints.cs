using SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

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
                CalculatePriceRequest request,
                CalculatePriceQueryHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    // Map request DTO to query with value objects
                    var query = new CalculatePriceQuery
                    {
                        CategoryCode = CategoryCode.Of(request.CategoryCode),
                        PickupDate = request.PickupDate,
                        ReturnDate = request.ReturnDate,
                        LocationCode = !string.IsNullOrWhiteSpace(request.LocationCode)
                            ? LocationCode.Of(request.LocationCode)
                            : null
                    };

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
            .WithDescription(
                "Calculates the total rental price for a vehicle category and period with German VAT (19%). Supports location-specific pricing if configured.")
            .Produces<PriceCalculationResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}

/// <summary>
///     Request DTO for calculating rental price.
/// </summary>
public sealed record CalculatePriceRequest
{
    /// <summary>
    ///     Vehicle category code (e.g., "KLEIN", "MITTEL", "SUV").
    /// </summary>
    public required string CategoryCode { get; init; }

    /// <summary>
    ///     Pickup date.
    /// </summary>
    public required DateTime PickupDate { get; init; }

    /// <summary>
    ///     Return date.
    /// </summary>
    public required DateTime ReturnDate { get; init; }

    /// <summary>
    ///     Optional location code for location-specific pricing.
    /// </summary>
    public string? LocationCode { get; init; }
}
