using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Domain event raised when a new vehicle is added to the fleet.
/// </summary>
public sealed record VehicleAddedToFleet(
    VehicleIdentifier VehicleId,
    VehicleName Name,
    VehicleCategory Category,
    Money DailyRate
) : DomainEvent;
