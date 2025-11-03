using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Domain event raised when a vehicle's daily rate is changed.
/// </summary>
public sealed record VehicleDailyRateChanged(
    VehicleIdentifier VehicleId,
    Money OldRate,
    Money NewRate
) : DomainEvent;
