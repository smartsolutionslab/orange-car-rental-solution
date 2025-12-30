using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Command to add a new vehicle to the fleet.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record AddVehicleToFleetCommand(
    VehicleName Name,
    VehicleCategory Category,
    LocationCode CurrentLocationCode,
    Money DailyRate,
    SeatingCapacity Seats,
    FuelType FuelType,
    TransmissionType TransmissionType,
    LicensePlate? LicensePlate,
    Manufacturer? Manufacturer,
    VehicleModel? Model,
    ManufacturingYear? Year,
    ImageUrl? ImageUrl) : ICommand<AddVehicleToFleetResult>;
