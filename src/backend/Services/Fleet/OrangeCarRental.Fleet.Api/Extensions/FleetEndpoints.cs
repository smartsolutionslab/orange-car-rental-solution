using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

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

        // Location endpoints
        var locations = app.MapGroup("/api/locations")
            .WithTags("Fleet - Locations")
            .WithOpenApi();

        // GET /api/locations - Get all locations
        locations.MapGet("/", async (
            GetLocationsQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetLocationsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetLocations")
        .WithSummary("Get all rental locations")
        .WithDescription("Returns all available rental locations in Germany.")
        .Produces<IReadOnlyList<LocationDto>>(StatusCodes.Status200OK);

        // GET /api/locations/{code} - Get location by code
        locations.MapGet("/{code}", async (
            string code,
            GetLocationByCodeQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetLocationByCodeQuery(code), cancellationToken);
            return result is not null
                ? Results.Ok(result)
                : Results.NotFound(new { message = $"Location with code '{code}' not found" });
        })
        .WithName("GetLocationByCode")
        .WithSummary("Get location by code")
        .WithDescription("Returns a specific location by its code (e.g., BER-HBF).")
        .Produces<LocationDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
