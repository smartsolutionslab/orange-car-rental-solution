using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Command to move a vehicle to a different location.
/// </summary>
public sealed record UpdateVehicleLocationCommand(
    VehicleIdentifier VehicleId,
    LocationCode NewLocationCode
) : ICommand<UpdateVehicleLocationResult>;
