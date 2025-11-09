using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
///     Query to search available vehicles based on filters.
///     Supports filtering by date range, location, category, and other criteria.
/// </summary>
public sealed record SearchVehiclesQuery(
    DateTime? PickupDate,
    DateTime? ReturnDate,
    string? LocationCode,
    string? CategoryCode,
    int? MinSeats,
    string? FuelType,
    string? TransmissionType,
    decimal? MaxDailyRateGross,
    int? PageNumber,
    int? PageSize


) : IQuery<SearchVehiclesResult>;
