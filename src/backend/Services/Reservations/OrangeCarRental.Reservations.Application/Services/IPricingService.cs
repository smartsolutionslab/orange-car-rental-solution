namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

/// <summary>
///     Service for calculating rental prices via the Pricing API.
/// </summary>
public interface IPricingService
{
    /// <summary>
    ///     Calculate the price for a vehicle rental.
    /// </summary>
    /// <param name="categoryCode">The vehicle category code (e.g., "KLEIN", "KOMPAKT")</param>
    /// <param name="pickupDate">The rental pickup date</param>
    /// <param name="returnDate">The rental return date</param>
    /// <param name="locationCode">Optional location code for location-specific pricing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Price calculation result</returns>
    Task<PriceCalculationDto> CalculatePriceAsync(
        string categoryCode,
        DateTime pickupDate,
        DateTime returnDate,
        string? locationCode = null,
        CancellationToken cancellationToken = default);
}
