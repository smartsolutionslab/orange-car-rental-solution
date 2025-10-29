using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a new vehicle is added to the fleet.
/// </summary>
public sealed record VehicleAddedToFleet(
    VehicleIdentifier VehicleId,
    VehicleName Name,
    VehicleCategory Category,
    Money DailyRate
) : DomainEvent;
