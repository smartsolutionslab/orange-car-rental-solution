using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;

/// <summary>
///     Command to add a new vehicle to the fleet.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record AddVehicleToFleetCommand
{
    public required VehicleName Name { get; init; }
    public required VehicleCategory Category { get; init; }
    public required Location CurrentLocation { get; init; }
    public required Money DailyRate { get; init; }
    public required SeatingCapacity Seats { get; init; }
    public required FuelType FuelType { get; init; }
    public required TransmissionType TransmissionType { get; init; }
    public string? LicensePlate { get; init; }
    public Manufacturer? Manufacturer { get; init; }
    public VehicleModel? Model { get; init; }
    public ManufacturingYear? Year { get; init; }
    public string? ImageUrl { get; init; }
}
