using SmartSolutionsLab.BuildingBlocks.Domain;
using SmartSolutionsLab.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle is moved to a different location.
/// </summary>
public sealed record VehicleLocationChanged(
    VehicleIdentifier VehicleId,
    Location OldLocation,
    Location NewLocation
) : DomainEvent;
