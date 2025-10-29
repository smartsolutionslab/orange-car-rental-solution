using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle is moved to a different location.
/// </summary>
public sealed record VehicleLocationChanged(
    VehicleIdentifier VehicleId,
    Location OldLocation,
    Location NewLocation
) : DomainEvent;
