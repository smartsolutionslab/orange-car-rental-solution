using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Parameters for searching vehicles in the repository.
/// </summary>
public sealed record VehicleSearchParameters(
    LocationCode? LocationCode,
    VehicleCategory? Category,
    SeatingCapacity? MinSeats,
    FuelType? FuelType,
    TransmissionType? TransmissionType,
    Money? MaxDailyRate,
    VehicleStatus? Status,
    SearchPeriod? Period,
    PagingInfo Paging,
    SortingInfo Sorting) : SearchParameters(Paging, Sorting);
