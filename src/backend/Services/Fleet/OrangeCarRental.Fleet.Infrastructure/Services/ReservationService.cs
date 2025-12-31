using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;
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
    private const string ServiceName = "Reservations API";

    public async Task<IReadOnlyList<VehicleIdentifier>> GetBookedVehicleIdsAsync(
        SearchPeriod period,
        CancellationToken cancellationToken = default)
    {
        var url = $"/api/reservations/availability?pickupDate={period.PickupDate:yyyy-MM-dd}&returnDate={period.ReturnDate:yyyy-MM-dd}";

        var result = await httpClient.GetJsonAsync<VehicleAvailabilityResponse>(
            url,
            ServiceName,
            cancellationToken);

        return result.BookedVehicleIds.Select(id => VehicleIdentifier.From(id)).ToArray();
    }

    private sealed record VehicleAvailabilityResponse(
        IReadOnlyList<Guid> BookedVehicleIds,
        DateOnly PickupDate,
        DateOnly ReturnDate);
}
