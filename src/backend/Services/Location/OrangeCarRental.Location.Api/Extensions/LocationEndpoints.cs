using SmartSolutionsLab.OrangeCarRental.Location.Application.Commands.CreateLocation;
using SmartSolutionsLab.OrangeCarRental.Location.Application.Queries.GetAllLocations;

namespace SmartSolutionsLab.OrangeCarRental.Location.Api.Extensions;

public static class LocationEndpoints
{
    public static IEndpointRouteBuilder MapLocationEndpoints(this IEndpointRouteBuilder app)
    {
        var locations = app.MapGroup("/api/locations")
            .WithTags("Locations")
            .WithOpenApi();

        // GET /api/locations - Get all active locations
        locations.MapGet("/", async (
            GetAllLocationsQueryHandler handler,
            CancellationToken cancellationToken) =>
            {
                var query = new GetAllLocationsQuery();
                var result = await handler.HandleAsync(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetAllLocations")
            .WithSummary("Get all active rental locations")
            .Produces<GetAllLocationsResult>()
            .AllowAnonymous();

        // POST /api/locations - Create new location
        locations.MapPost("/", async (
            CreateLocationCommand command,
            CreateLocationCommandHandler handler,
            CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Created($"/api/locations/{result.LocationId}", result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while creating the location");
                }
            })
            .WithName("CreateLocation")
            .WithSummary("Create a new rental location")
            .Produces<CreateLocationResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("FleetManagerOrAdminPolicy");

        return app;
    }
}
