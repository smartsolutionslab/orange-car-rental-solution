using System.Text;
using System.Text.Json;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

/// <summary>
///     HTTP client implementation for calling the Pricing API.
/// </summary>
public sealed class PricingService : IPricingService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public PricingService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<PriceCalculationDto> CalculatePriceAsync(
        string categoryCode,
        DateTime pickupDate,
        DateTime returnDate,
        string? locationCode = null,
        CancellationToken cancellationToken = default)
    {
        // Prepare request payload
        var request = new
        {
            CategoryCode = categoryCode,
            PickupDate = pickupDate,
            ReturnDate = returnDate,
            LocationCode = locationCode
        };

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Call Pricing API
        var response = await _httpClient.PostAsync("/api/pricing/calculate", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Failed to calculate price from Pricing API. Status: {response.StatusCode}, Error: {errorContent}");
        }

        // Deserialize response
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PriceCalculationDto>(responseJson, _jsonOptions);

        if (result is null)
            throw new InvalidOperationException("Failed to deserialize price calculation response from Pricing API");

        return result;
    }
}
