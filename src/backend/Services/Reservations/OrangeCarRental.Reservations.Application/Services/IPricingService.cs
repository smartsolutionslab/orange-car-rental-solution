using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

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
