using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

public static class MappingExtensions
{
    public static VehicleDto ToDto(this Vehicle vehicle) => new()
    {
        Id = vehicle.Id.Value,
        Name = vehicle.Name.Value,
        CategoryCode = vehicle.Category.Code,
        CategoryName = vehicle.Category.Name,
        LocationCode = vehicle.CurrentLocation.Code.Value,
        City = vehicle.CurrentLocation.Address.City.Value,
        Seats = vehicle.Seats.Value,
        FuelType = vehicle.FuelType.ToString(),
        TransmissionType = vehicle.TransmissionType.ToString(),
        DailyRateNet = vehicle.DailyRate.NetAmount,
        DailyRateVat = vehicle.DailyRate.VatAmount,
        DailyRateGross = vehicle.DailyRate.GrossAmount,
        Currency = vehicle.DailyRate.Currency.Code,
        Status = vehicle.Status.ToString(),
        LicensePlate = vehicle.LicensePlate,
        Manufacturer = vehicle.Manufacturer?.Value,
        Model = vehicle.Model?.Value,
        Year = vehicle.Year?.Value,
        ImageUrl = vehicle.ImageUrl
    };

    public static SearchVehiclesResult ToDto(this PagedResult<Vehicle> pagedResult)
    {
        return new()
        {
            Vehicles = pagedResult.Items.Select(vehicle => vehicle.ToDto()).ToList(),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };
    }

    public static VehicleSearchParameters ToVehicleSearchParameters(this SearchVehiclesQuery query)
    {
        return new VehicleSearchParameters
        {
            LocationCode = query.LocationCode,
            CategoryCode = query.CategoryCode,
            MinSeats = query.MinSeats,
            FuelType = query.FuelType.TryParseFuelType(),
            TransmissionType = query.TransmissionType.TryParseTransmissionType(),
            MaxDailyRateGross = query.MaxDailyRateGross,
            Status = VehicleStatus.Available, // Always filter to available vehicles
            PickupDate = query.PickupDate,
            ReturnDate = query.ReturnDate,
            PageNumber = query.PageNumber ?? 1,
            PageSize = query.PageSize ?? 20
        };
    }
}
