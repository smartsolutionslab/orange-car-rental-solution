using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Parameters for searching vehicles in the repository.
/// </summary>
public sealed record VehicleSearchParameters : SearchParameters
{
    public LocationCode? LocationCode { get; init; }
    public VehicleCategory? Category { get; init; }
    public SeatingCapacity? MinSeats { get; init; }
    public FuelType? FuelType { get; init; }
    public TransmissionType? TransmissionType { get; init; }
    public Money? MaxDailyRate { get; init; }
    public VehicleStatus? Status { get; init; }
    public SearchPeriod? Period { get; init; }

    public VehicleSearchParameters(PagingInfo paging) : base(paging, SortingInfo.None)
    {
    }

    public VehicleSearchParameters(PagingInfo paging, SortingInfo sorting) : base(paging, sorting)
    {
    }

    /// <summary>
    ///     Creates search parameters with default paging.
    /// </summary>
    public static VehicleSearchParameters Default() => new(PagingInfo.Default);

    /// <summary>
    ///     Creates search parameters with specific paging.
    /// </summary>
    public static VehicleSearchParameters WithPaging(int pageNumber, int pageSize) =>
        new(PagingInfo.Create(pageNumber, pageSize));

    /// <summary>
    ///     Creates search parameters filtered by location.
    /// </summary>
    public static VehicleSearchParameters ForLocation(LocationCode locationCode, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { LocationCode = locationCode };

    /// <summary>
    ///     Creates search parameters filtered by category.
    /// </summary>
    public static VehicleSearchParameters ForCategory(VehicleCategory category, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { Category = category };

    /// <summary>
    ///     Creates search parameters filtered by fuel type.
    /// </summary>
    public static VehicleSearchParameters ForFuelType(FuelType fuelType, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { FuelType = fuelType };

    /// <summary>
    ///     Creates search parameters filtered by minimum seats.
    /// </summary>
    public static VehicleSearchParameters WithMinSeats(SeatingCapacity minSeats, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { MinSeats = minSeats };

    /// <summary>
    ///     Creates search parameters filtered by maximum daily rate.
    /// </summary>
    public static VehicleSearchParameters WithMaxRate(Money maxDailyRate, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { MaxDailyRate = maxDailyRate };

    /// <summary>
    ///     Creates search parameters filtered by status.
    /// </summary>
    public static VehicleSearchParameters ForStatus(VehicleStatus status, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { Status = status };

    /// <summary>
    ///     Creates search parameters with multiple filters.
    /// </summary>
    public static VehicleSearchParameters WithFilters(
        LocationCode? locationCode = null,
        FuelType? fuelType = null,
        SeatingCapacity? minSeats = null,
        PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default)
        {
            LocationCode = locationCode,
            FuelType = fuelType,
            MinSeats = minSeats
        };
}
