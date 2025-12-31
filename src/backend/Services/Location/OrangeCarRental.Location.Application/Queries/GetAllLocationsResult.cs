using SmartSolutionsLab.OrangeCarRental.Location.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Queries;

public sealed record GetAllLocationsResult
{
    public required IReadOnlyList<LocationDto> Locations { get; init; }
}
