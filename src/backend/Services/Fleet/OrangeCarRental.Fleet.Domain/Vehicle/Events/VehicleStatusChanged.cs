using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle.Events;

/// <summary>
///     Domain event raised when a vehicle's status changes.
/// </summary>
public sealed record VehicleStatusChanged(
    VehicleIdentifier VehicleId,
    VehicleStatus NewStatus
) : DomainEvent;
