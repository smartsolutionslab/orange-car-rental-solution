using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Extensions;

public static class FleetEndpoints
{
    public static IEndpointRouteBuilder MapFleetEndpoints(this IEndpointRouteBuilder app)
    {
        var fleet = app.MapGroup("/api/vehicles")
            .WithTags("Fleet - Vehicles")
            .WithOpenApi();

        // GET /api/vehicles - Search vehicles
        fleet.MapGet("/", async (
            [AsParameters] SearchVehiclesQuery query,
            SearchVehiclesQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("SearchVehicles")
        .WithSummary("Search available vehicles")
        .WithDescription("Search and filter vehicles by location, category, dates, and other criteria. Returns vehicles with German VAT-inclusive pricing (19%).")
        .Produces<SearchVehiclesResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}
