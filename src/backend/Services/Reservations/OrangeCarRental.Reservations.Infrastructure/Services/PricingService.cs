using System.Text;
using System.Text.Json;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

/// <summary>
///     HTTP client implementation for calling the Pricing API.
/// </summary>
public sealed class PricingService(HttpClient httpClient) : IPricingService
{
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PriceCalculationDto> CalculatePriceAsync(
        VehicleCategory category,
        BookingPeriod period,
        LocationCode? location = null,
        CancellationToken cancellationToken = default)
    {
        // Prepare request payload
        var request = new
        {
            CategoryCode = category.Code,
            PickupDate = period.PickupDate,
            ReturnDate = period.ReturnDate,
            LocationCode = location?.Value ?? string.Empty
        };

        var json = JsonSerializer.Serialize(request, jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Call Pricing API
        var response = await httpClient.PostAsync("/api/pricing/calculate", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to calculate price from Pricing API. Status: {response.StatusCode}, Error: {errorContent}");
        }

        // Deserialize response
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PriceCalculationDto>(responseJson, jsonOptions) ?? throw new InvalidOperationException("Failed to deserialize price calculation response from Pricing API");
        return result;
    }
}
