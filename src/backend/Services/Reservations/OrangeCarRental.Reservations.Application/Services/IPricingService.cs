using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

/// <summary>
///     Service for calculating rental prices via the Pricing API.
/// </summary>
public interface IPricingService
{
    Task<PriceCalculationDto> CalculatePriceAsync(
        VehicleCategory category,
        BookingPeriod period,
        LocationCode? location = null,
        CancellationToken cancellationToken = default);
}
