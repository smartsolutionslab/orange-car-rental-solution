using OrangeCarRental.BuildingBlocks.Domain;
using OrangeCarRental.Fleet.Domain.ValueObjects;

namespace OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle is moved to a different location.
/// </summary>
public sealed record VehicleLocationChanged(
    VehicleId VehicleId,
    Location OldLocation,
    Location NewLocation
) : DomainEvent;
