using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Api.Contracts;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddLocation;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.ChangeLocationStatus;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateLocation;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleStatus;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleDailyRate;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
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
        fleet.MapGet("/", async ([AsParameters] SearchVehiclesQuery query, SearchVehiclesQueryHandler handler, CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("SearchVehicles")
            .WithSummary("Search available vehicles")
            .WithDescription("Search and filter vehicles by location, category, dates, and other criteria. Returns vehicles with German VAT-inclusive pricing (19%).")
            .Produces<SearchVehiclesResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        // POST /api/vehicles - Add new vehicle to fleet
        fleet.MapPost("/", async (AddVehicleToFleetRequest request, AddVehicleToFleetCommandHandler handler, CancellationToken cancellationToken) =>
            {
                var (basicInfo, specifications, locationAndPricing, registration) = request;

                try
                {
                    var command = new AddVehicleToFleetCommand(
                        VehicleName.From(basicInfo.Name),
                        VehicleCategory.From(specifications.Category),
                        LocationCode.From(locationAndPricing.LocationCode),
                        Money.Euro(locationAndPricing.DailyRateNet),
                        SeatingCapacity.From(specifications.Seats),
                        specifications.FuelType.ParseFuelType(),
                        specifications.TransmissionType.ParseTransmissionType(),
                        registration?.LicensePlate,
                        Manufacturer.FromNullable(basicInfo.Manufacturer),
                        VehicleModel.FromNullable(basicInfo.Model),
                        ManufacturingYear.FromNullable(basicInfo.Year),
                        basicInfo.ImageUrl
                    );

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
            .WithDescription("Adds a new vehicle to the rental fleet. Daily rate is net amount in EUR, 19% German VAT will be calculated automatically. Vehicle will be available for rental immediately.")
            .Produces<AddVehicleToFleetResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("FleetManagerOrAdminPolicy");

        // PUT /api/vehicles/{id}/status - Update vehicle status
        fleet.MapPut("/{id:guid}/status", async (Guid id, string status, UpdateVehicleStatusCommandHandler handler, CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new UpdateVehicleStatusCommand(
                        VehicleIdentifier.From(id),
                        status.Parse());
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while updating the vehicle status");
                }
            })
            .WithName("UpdateVehicleStatus")
            .WithSummary("Update vehicle status")
            .WithDescription("Updates a vehicle's operational status. Valid statuses: Available, Rented, Maintenance, OutOfService, Reserved. Cannot move a rented vehicle.")
            .Produces<UpdateVehicleStatusResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("FleetManagerOrAdminPolicy");

        // PUT /api/vehicles/{id}/location - Update vehicle location
        fleet.MapPut("/{id:guid}/location", async (Guid id, string locationCode, UpdateVehicleLocationCommandHandler handler, CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new UpdateVehicleLocationCommand(
                        VehicleIdentifier.From(id),
                        LocationCode.From(locationCode));
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while updating the vehicle location");
                }
            })
            .WithName("UpdateVehicleLocation")
            .WithSummary("Move vehicle to a different location")
            .WithDescription("Moves a vehicle to a different rental location. Vehicle must not be rented. Valid location codes: BER-HBF, MUC-FLG, FRA-FLG, HAM-HBF, CGN-HBF.")
            .Produces<UpdateVehicleLocationResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("FleetManagerOrAdminPolicy");

        // PUT /api/vehicles/{id}/daily-rate - Update vehicle daily rate
        fleet.MapPut("/{id:guid}/daily-rate", async (Guid id, decimal dailyRateNet, UpdateVehicleDailyRateCommandHandler handler, CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new UpdateVehicleDailyRateCommand(
                        VehicleIdentifier.From(id),
                        Money.Euro(dailyRateNet)
                    );
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while updating the daily rate");
                }
            })
            .WithName("UpdateVehicleDailyRate")
            .WithSummary("Update vehicle daily rental rate")
            .WithDescription(
                "Updates a vehicle's daily rental rate. Provide the net amount in EUR, 19% German VAT will be calculated automatically.")
            .Produces<UpdateVehicleDailyRateResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("FleetManagerOrAdminPolicy");

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
            .Produces<IReadOnlyList<LocationDto>>()
            .AllowAnonymous();

        // GET /api/locations/{code} - Get location by code
        locations.MapGet("/{code}", async (string code, GetLocationByCodeQueryHandler handler, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(
                        new GetLocationByCodeQuery(
                            LocationCode.From(code)
                        ),
                        cancellationToken);
                    return Results.Ok(result);
                }
                catch (EntityNotFoundException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("GetLocationByCode")
            .WithSummary("Get location by code")
            .WithDescription("Returns a specific location by its code (e.g., BER-HBF).")
            .Produces<LocationDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // POST /api/locations - Add new location
        locations.MapPost("/", async (
                string code,
                string name,
                string street,
                string city,
                string postalCode,
                AddLocationCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new AddLocationCommand(
                        LocationCode.From(code),
                        LocationName.From(name),
                        Address.Of(
                            Street.From(street),
                            City.From(city),
                            PostalCode.From(postalCode))
                    );

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Created($"/api/locations/{result.Code}", result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("AddLocation")
            .WithSummary("Add a new rental location")
            .WithDescription("Creates a new rental location. Location code must be unique.")
            .Produces<AddLocationResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .RequireAuthorization("AdminPolicy");

        // PUT /api/locations/{code} - Update location information
        locations.MapPut("/{code}", async (
                string code,
                string name,
                string street,
                string city,
                string postalCode,
                UpdateLocationCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new UpdateLocationCommand(
                        LocationCode.From(code),
                        LocationName.From(name),
                        Address.Of(
                            Street.From(street),
                            City.From(city),
                            PostalCode.From(postalCode))
                    );

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (EntityNotFoundException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("UpdateLocation")
            .WithSummary("Update location information")
            .WithDescription("Updates the name and address of a location by its code.")
            .Produces<UpdateLocationResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization("AdminPolicy");

        // PATCH /api/locations/{code}/status - Change location status
        locations.MapPatch("/{code}/status", async (
                string code,
                string status,
                string? reason,
                ChangeLocationStatusCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new ChangeLocationStatusCommand(
                        LocationCode.From(code),
                        status.ParseLocationStatus(),
                        StatusChangeReason.FromNullable(reason)
                    );

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (EntityNotFoundException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("ChangeLocationStatus")
            .WithSummary("Change location status")
            .WithDescription("Changes the status of a location by its code (Active, Closed, UnderMaintenance, Inactive).")
            .Produces<ChangeLocationStatusResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization("AdminPolicy");

        return app;
    }
}
