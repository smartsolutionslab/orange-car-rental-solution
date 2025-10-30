using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

public static class MappingExtensions
{
    public static VehicleDto ToDto(this Vehicle vehicle) => new()
    {
        Id = vehicle.Id.Value,
        Name = vehicle.Name.Value,
        CategoryCode = vehicle.Category.Code,
        CategoryName = vehicle.Category.Name,
        LocationCode = vehicle.CurrentLocation.Code,
        City = vehicle.CurrentLocation.City,
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
}
