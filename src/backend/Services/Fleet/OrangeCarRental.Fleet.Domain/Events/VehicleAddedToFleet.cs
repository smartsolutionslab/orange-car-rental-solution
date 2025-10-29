using SmartSolutionsLab.BuildingBlocks.Domain;
using SmartSolutionsLab.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a new vehicle is added to the fleet.
/// </summary>
public sealed record VehicleAddedToFleet(
    VehicleIdentifier VehicleId,
    VehicleName Name,
    VehicleCategory Category,
    Money DailyRate
) : DomainEvent;
