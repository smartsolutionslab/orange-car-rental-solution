using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Domain event raised when a vehicle's status changes.
/// </summary>
public sealed record VehicleStatusChanged(
    VehicleIdentifier VehicleId,
    VehicleStatus NewStatus
) : DomainEvent;
