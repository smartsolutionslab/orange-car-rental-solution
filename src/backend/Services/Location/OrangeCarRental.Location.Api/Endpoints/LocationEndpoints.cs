using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Location.Api.Requests;
using SmartSolutionsLab.OrangeCarRental.Location.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Location.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Api.Endpoints;

public static class LocationEndpoints
{
    public static IEndpointRouteBuilder MapLocationEndpoints(this IEndpointRouteBuilder app)
    {
        var locations = app.MapGroup("/api/locations")
            .WithTags("Locations");

        // GET /api/locations - Get all active locations
        locations.MapGet("/", async (
            IQueryHandler<GetAllLocationsQuery, GetAllLocationsResult> handler,
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
            CreateLocationRequest request,
            ICommandHandler<CreateLocationCommand, CreateLocationResult> handler,
            CancellationToken cancellationToken) =>
            {
                try
                {
                    // Create value objects at API boundary
                    GeoCoordinates? coordinates = null;
                    if (request.Latitude.HasValue && request.Longitude.HasValue)
                    {
                        coordinates = GeoCoordinates.Of(request.Latitude.Value, request.Longitude.Value);
                    }

                    var command = new CreateLocationCommand(
                        LocationCode.From(request.Code),
                        LocationName.From(request.Name),
                        LocationAddress.Of(request.Street, request.City, request.PostalCode),
                        ContactInfo.Of(request.Phone, request.Email),
                        OpeningHours.From(request.OpeningHours),
                        coordinates);

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Created($"/api/locations/{result.Code}", result);
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
