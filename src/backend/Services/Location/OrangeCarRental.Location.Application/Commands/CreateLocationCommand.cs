using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Commands;

/// <summary>
///     Command to create a new rental location.
/// </summary>
public sealed record CreateLocationCommand(
    LocationCode Code,
    LocationName Name,
    LocationAddress Address,
    ContactInfo Contact,
    OpeningHours OpeningHours,
    GeoCoordinates? Coordinates = null) : ICommand<CreateLocationResult>;
