using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Events;

/// <summary>
/// Domain event raised when a vehicle's daily rate is changed.
/// </summary>
public sealed record VehicleDailyRateChanged(
    VehicleIdentifier VehicleId,
    Money OldRate,
    Money NewRate
) : DomainEvent;
