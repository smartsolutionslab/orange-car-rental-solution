using SmartSolutionsLab.BuildingBlocks.Domain;
using SmartSolutionsLab.Fleet.Domain.Enums;
using SmartSolutionsLab.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle's status changes.
/// </summary>
public sealed record VehicleStatusChanged(
    VehicleIdentifier VehicleId,
    VehicleStatus NewStatus
) : DomainEvent;
