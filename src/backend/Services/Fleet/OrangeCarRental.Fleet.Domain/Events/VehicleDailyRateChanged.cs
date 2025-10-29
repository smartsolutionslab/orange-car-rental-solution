using SmartSolutionsLab.BuildingBlocks.Domain;
using SmartSolutionsLab.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle's daily rate is changed.
/// </summary>
public sealed record VehicleDailyRateChanged(
    VehicleIdentifier VehicleId,
    Money OldRate,
    Money NewRate
) : DomainEvent;
