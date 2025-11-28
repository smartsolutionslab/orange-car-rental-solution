using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.ChangeLocationStatus;

/// <summary>
///     Command to change location status.
/// </summary>
public sealed record ChangeLocationStatusCommand(
    LocationCode Code,
    LocationStatus NewStatus,
    string? Reason = null
) : ICommand<ChangeLocationStatusResult>;
