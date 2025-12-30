using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;

/// <summary>
///     Extension methods for mapping between domain objects and DTOs.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Vehicle mapping.
    /// </summary>
    extension(Vehicle vehicle)
    {
        /// <summary>
        ///     Maps a Vehicle aggregate to a VehicleDto.
        /// </summary>
        public VehicleDto ToDto() => new(
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
    }

    /// <summary>
    ///     C# 14 Extension Members for SearchVehiclesQuery mapping.
    /// </summary>
    extension(SearchVehiclesQuery query)
    {
        /// <summary>
        ///     Maps a SearchVehiclesQuery to VehicleSearchParameters.
        ///     Direct mapping since query already uses value objects.
        /// </summary>
        public VehicleSearchParameters ToVehicleSearchParameters()
        {
            return new VehicleSearchParameters(
                query.LocationCode,
                query.Category,
                query.MinSeats,
                query.FuelType,
                query.TransmissionType,
                query.MaxDailyRate,
                VehicleStatus.Available, // Always filter to available vehicles
                query.Period,
                query.Paging,
                query.Sorting);
        }
    }
}
