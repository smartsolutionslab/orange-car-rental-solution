using System.Text.Json;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Services;

/// <summary>
///     HTTP client implementation for calling the Reservations API.
///     Replaces direct database access to maintain bounded context boundaries.
/// </summary>
public sealed class ReservationService(HttpClient httpClient) : IReservationService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
        DateOnly pickupDate,
        DateOnly returnDate,
        CancellationToken cancellationToken = default)
    {
        // Call Reservations API availability endpoint
        var url = $"/api/reservations/availability?pickupDate={pickupDate:yyyy-MM-dd}&returnDate={returnDate:yyyy-MM-dd}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Failed to get vehicle availability from Reservations API. Status: {response.StatusCode}, Error: {errorContent}");
        }

        // Deserialize response
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<VehicleAvailabilityResponse>(responseJson, _jsonOptions);

        return result?.BookedVehicleIds ?? Array.Empty<Guid>();
    }

    /// <summary>
    ///     Response DTO matching the Reservations API GetVehicleAvailabilityResult.
    /// </summary>
    private sealed record VehicleAvailabilityResponse(
        IReadOnlyList<Guid> BookedVehicleIds,
        DateOnly PickupDate,
        DateOnly ReturnDate);
}
