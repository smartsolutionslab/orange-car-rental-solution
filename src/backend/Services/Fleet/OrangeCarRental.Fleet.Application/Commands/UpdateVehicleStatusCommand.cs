using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Command to update a vehicle's status.
/// </summary>
public sealed record UpdateVehicleStatusCommand(
    VehicleIdentifier VehicleId,
    VehicleStatus NewStatus
) : ICommand<UpdateVehicleStatusResult>;
