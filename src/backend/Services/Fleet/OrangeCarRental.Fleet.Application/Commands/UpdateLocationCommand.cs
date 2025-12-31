using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Command to update location information.
/// </summary>
public sealed record UpdateLocationCommand(
    LocationCode Code,
    LocationName Name,
    Address Address
) : ICommand<UpdateLocationResult>;
