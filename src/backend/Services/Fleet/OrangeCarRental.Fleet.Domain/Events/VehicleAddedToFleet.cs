using OrangeCarRental.BuildingBlocks.Domain;
using OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using OrangeCarRental.Fleet.Domain.ValueObjects;

namespace OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a new vehicle is added to the fleet.
/// </summary>
public sealed record VehicleAddedToFleet(
    VehicleId VehicleId,
    VehicleName Name,
    VehicleCategory Category,
    Money DailyRate
) : DomainEvent;
