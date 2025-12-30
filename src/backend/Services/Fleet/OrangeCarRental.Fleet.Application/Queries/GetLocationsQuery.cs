using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries;

/// <summary>
///     Query to get all available rental locations
/// </summary>
public sealed record GetLocationsQuery : IQuery<IReadOnlyList<LocationDto>>;
