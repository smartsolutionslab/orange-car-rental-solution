using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Api.Contracts;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

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
            .WithDescription(
                "Search and filter vehicles by location, category, dates, and other criteria. Returns vehicles with German VAT-inclusive pricing (19%).")
            .Produces<SearchVehiclesResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /api/vehicles - Add new vehicle to fleet
        fleet.MapPost("/", async (
                AddVehicleToFleetRequest request,
                AddVehicleToFleetCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    // Map request DTO to command with value objects
                    var command = new AddVehicleToFleetCommand
                    {
                        Name = VehicleName.Of(request.Name),
                        Category = VehicleCategory.FromCode(request.Category),
                        CurrentLocation = Location.FromCode(LocationCode.Of(request.LocationCode)),
                        DailyRate = Money.Euro(request.DailyRateNet),
                        Seats = SeatingCapacity.Of(request.Seats),
                        FuelType = request.FuelType.ParseFuelType(),
                        TransmissionType = request.TransmissionType.ParseTransmissionType(),
                        LicensePlate = request.LicensePlate,
                        Manufacturer = request.Manufacturer is not null ? Manufacturer.Of(request.Manufacturer) : null,
                        Model = request.Model is not null ? VehicleModel.Of(request.Model) : null,
                        Year = request.Year.HasValue ? ManufacturingYear.Of(request.Year.Value) : null,
                        ImageUrl = request.ImageUrl
                    };

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Created($"/api/vehicles/{result.VehicleId}", result);
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
                        title: "An error occurred while adding the vehicle to the fleet");
                }
            })
            .WithName("AddVehicleToFleet")
            .WithSummary("Add a new vehicle to the fleet")
            .WithDescription(
                "Adds a new vehicle to the rental fleet. Daily rate is net amount in EUR, 19% German VAT will be calculated automatically. Vehicle will be available for rental immediately.")
            .Produces<AddVehicleToFleetResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

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
            .Produces<IReadOnlyList<LocationDto>>();

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
            .Produces<LocationDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
