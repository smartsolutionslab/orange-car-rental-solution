using System.Text.Json;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Services;

/// <summary>
///     HTTP client implementation for calling the Reservations API.
///     Replaces direct database access to maintain bounded context boundaries.
/// </summary>
public sealed class ReservationService(HttpClient httpClient) : IReservationService
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<VehicleIdentifier>> GetBookedVehicleIdsAsync(
        SearchPeriod period,
        CancellationToken cancellationToken = default)
    {
        // Call Reservations API availability endpoint
        var url = $"/api/reservations/availability?pickupDate={period.PickupDate:yyyy-MM-dd}&returnDate={period.ReturnDate:yyyy-MM-dd}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to get vehicle availability from Reservations API. Status: {response.StatusCode}, Error: {errorContent}");
        }

        // Deserialize response
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<VehicleAvailabilityResponse>(responseJson, jsonOptions);

        return result?.BookedVehicleIds.Select(id => VehicleIdentifier.From(id)).ToArray() ?? [];
    }

    /// <summary>
    ///     Response DTO matching the Reservations API GetVehicleAvailabilityResult.
    /// </summary>
    private sealed record VehicleAvailabilityResponse(
        IReadOnlyList<Guid> BookedVehicleIds,
        DateOnly PickupDate,
        DateOnly ReturnDate);
}
