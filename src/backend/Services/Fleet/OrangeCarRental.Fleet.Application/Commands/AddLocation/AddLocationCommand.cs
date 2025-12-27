using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddLocation;

/// <summary>
///     Command to add a new location.
/// </summary>
public sealed record AddLocationCommand(
    LocationCode Code,
    LocationName Name,
    Address Address
) : ICommand<AddLocationResult>;
