using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Parameters for searching vehicles in the repository.
/// </summary>
public sealed record VehicleSearchParameters(
    LocationCode? LocationCode,
    VehicleCategory? Category,
    int? MinSeats,
    FuelType? FuelType,
    TransmissionType? TransmissionType,
    decimal? MaxDailyRateGross,
    VehicleStatus? Status,
    SearchPeriod? Period,
    int PageNumber,
    int PageSize) : SearchParameters(PageNumber, PageSize, null, false)
{
    public VehicleSearchParameters(int pageNumber, int pageSize) : this(null, null, null, null, null, null, null, null,
        pageNumber, pageSize)
    {}

    public VehicleSearchParameters(LocationCode? locationCode, int pageNumber, int pageSize) : this(locationCode, null, null,null, null, null, null, null, pageNumber, pageSize)
    {}

    public VehicleSearchParameters(VehicleCategory? category, int pageNumber, int pageSize) : this(null, category, null, null, null, null, null, null, pageNumber, pageSize)
    {}

    public VehicleSearchParameters(FuelType? fuelType, int pageNumber, int pageSize) : this(null, null, null, fuelType, null, null, null, null, pageNumber, pageSize)
    {}

    public VehicleSearchParameters(int? minSeats, int pageNumber, int pageSize) : this(null, null, minSeats, null, null,null, null, null, pageNumber, pageSize)
    {}

    public VehicleSearchParameters(decimal? maxDailyRateGross, int pageNumber, int pageSize) : this(null, null, null, null, null, maxDailyRateGross, null, null, pageNumber, pageSize)
    {}

    public VehicleSearchParameters(LocationCode? locationCode, FuelType? fuelType, int? minSeats, int pageNumber, int pageSize) : this(locationCode, null, minSeats, fuelType, null, null, null, null, pageNumber, pageSize)
    {}

    public VehicleSearchParameters(VehicleStatus? status, int pageNumber, int pageSize): this(null, null, null, null, null, null, status, null, pageNumber, pageSize)
    {}
}
