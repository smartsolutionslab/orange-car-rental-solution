using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries;

/// <summary>
///     Query to search available vehicles based on filters.
///     Uses value objects for type-safe filtering.
/// </summary>
public sealed record SearchVehiclesQuery(
    SearchPeriod? Period,
    LocationCode? LocationCode,
    VehicleCategory? Category,
    SeatingCapacity? MinSeats,
    FuelType? FuelType,
    TransmissionType? TransmissionType,
    Money? MaxDailyRate,
    PagingInfo Paging,
    SortingInfo Sorting
) : IQuery<PagedResult<VehicleDto>>;
