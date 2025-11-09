using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;

/// <summary>
///     Command to add a new vehicle to the fleet.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record AddVehicleToFleetCommand(
    VehicleName Name,
    VehicleCategory Category,
    Location CurrentLocation,
    Money DailyRate,
    SeatingCapacity Seats,
    FuelType FuelType,
    TransmissionType TransmissionType,
    string? LicensePlate,
    Manufacturer? Manufacturer,
    VehicleModel? Model,
    ManufacturingYear? Year,
    string? ImageUrl) : ICommand<AddVehicleToFleetResult>;
