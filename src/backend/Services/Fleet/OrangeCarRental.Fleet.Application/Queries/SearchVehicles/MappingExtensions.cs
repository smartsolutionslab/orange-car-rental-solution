using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

public static class MappingExtensions
{
    public static VehicleDto ToDto(this Vehicle vehicle) => new(
        vehicle.Id.Value,
        vehicle.Name.Value,
        vehicle.Category.Code,
        vehicle.Category.Name,
        vehicle.CurrentLocationCode.Value,
        string.Empty, // City will be fetched separately if needed
        vehicle.Seats.Value,
        vehicle.FuelType.ToString(),
        vehicle.TransmissionType.ToString(),
        vehicle.DailyRate.NetAmount,
        vehicle.DailyRate.VatAmount,
        vehicle.DailyRate.GrossAmount,
        vehicle.DailyRate.Currency.Code,
        vehicle.Status.ToString(),
        vehicle.LicensePlate,
        vehicle.Manufacturer?.Value,
        vehicle.Model?.Value,
        vehicle.Year?.Value,
        vehicle.ImageUrl);

    public static SearchVehiclesResult ToDto(this PagedResult<Vehicle> pagedResult)
    {
        return new SearchVehiclesResult(
            pagedResult.Items.Select(vehicle => vehicle.ToDto()).ToList(),
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalPages);
    }

    public static VehicleSearchParameters ToVehicleSearchParameters(this SearchVehiclesQuery query)
    {
        return new VehicleSearchParameters(
            LocationCode.FromNullable(query.LocationCode),
            VehicleCategory.FromNullable(query.CategoryCode),
            query.MinSeats.HasValue ? SeatingCapacity.From(query.MinSeats.Value) : null,
            query.FuelType.TryParseFuelType(),
            query.TransmissionType.TryParseTransmissionType(),
            query.MaxDailyRateGross.HasValue
                ? Money.EuroGross(query.MaxDailyRateGross.Value)
                : null,
            VehicleStatus.Available, // Always filter to available vehicles
            SearchPeriod.Of(query.PickupDate, query.ReturnDate),
            PagingInfo.Create(query.PageNumber ?? 1, query.PageSize ?? PagingInfo.DefaultPageSize),
            SortingInfo.Create());
    }
}
