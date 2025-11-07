using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Query to get all available rental locations
/// </summary>
public sealed record GetLocationsQuery : IQuery<IReadOnlyList<LocationDto>>;
