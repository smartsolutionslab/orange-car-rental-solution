using OrangeCarRental.BuildingBlocks.Domain;
using OrangeCarRental.Fleet.Domain.Enums;
using OrangeCarRental.Fleet.Domain.ValueObjects;

namespace OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle's status changes.
/// </summary>
public sealed record VehicleStatusChanged(
    VehicleId VehicleId,
    VehicleStatus NewStatus
) : DomainEvent;
