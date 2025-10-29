using OrangeCarRental.BuildingBlocks.Domain;
using OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using OrangeCarRental.Fleet.Domain.ValueObjects;

namespace OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle's daily rate is changed.
/// </summary>
public sealed record VehicleDailyRateChanged(
    VehicleId VehicleId,
    Money OldRate,
    Money NewRate
) : DomainEvent;
