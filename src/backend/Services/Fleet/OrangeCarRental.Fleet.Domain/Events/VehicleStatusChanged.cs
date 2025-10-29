using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle's status changes.
/// </summary>
public sealed record VehicleStatusChanged(
    VehicleIdentifier VehicleId,
    VehicleStatus NewStatus
) : DomainEvent;
