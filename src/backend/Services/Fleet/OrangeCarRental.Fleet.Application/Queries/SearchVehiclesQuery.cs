using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries;

/// <summary>
///     Query to search available vehicles based on filters.
///     Supports filtering by date range, location, category, and other criteria.
/// </summary>
public sealed record SearchVehiclesQuery(
    DateOnly? PickupDate,
    DateOnly? ReturnDate,
    string? LocationCode,
    string? CategoryCode,
    int? MinSeats,
    string? FuelType,
    string? TransmissionType,
    decimal? MaxDailyRateGross,
    int? PageNumber,
    int? PageSize
) : IQuery<PagedResult<VehicleDto>>;
