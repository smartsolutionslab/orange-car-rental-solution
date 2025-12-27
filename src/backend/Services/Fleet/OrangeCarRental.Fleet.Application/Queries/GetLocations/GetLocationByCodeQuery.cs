using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Query to get a specific location by code
/// </summary>
public sealed record GetLocationByCodeQuery(LocationCode Code) : IQuery<LocationDto>;
