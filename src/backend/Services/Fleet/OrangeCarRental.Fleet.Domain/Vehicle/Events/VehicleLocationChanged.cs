using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle.Events;

/// <summary>
///     Domain event raised when a vehicle is moved to a different location.
/// </summary>
public sealed record VehicleLocationChanged(
    VehicleIdentifier VehicleId,
    LocationCode OldLocationCode,
    LocationCode NewLocationCode
) : DomainEvent;
