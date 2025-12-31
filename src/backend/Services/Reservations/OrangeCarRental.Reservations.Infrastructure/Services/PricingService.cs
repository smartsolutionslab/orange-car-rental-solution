using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

/// <summary>
///     HTTP client implementation for calling the Pricing API.
/// </summary>
public sealed class PricingService(HttpClient httpClient) : IPricingService
{
    private const string ServiceName = "Pricing API";

    public async Task<PriceCalculationDto> CalculatePriceAsync(
        VehicleCategory category,
        BookingPeriod period,
        LocationCode? location = null,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            CategoryCode = category.Code,
            PickupDate = period.PickupDate,
            ReturnDate = period.ReturnDate,
            LocationCode = location?.Value ?? string.Empty
        };

        return await httpClient.PostJsonAsync<object, PriceCalculationDto>(
            "/api/pricing/calculate",
            request,
            ServiceName,
            cancellationToken);
    }
}
