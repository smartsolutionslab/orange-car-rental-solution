namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;

/// <summary>
///     Service for communicating with the Reservations API.
///     Provides vehicle availability information from the Reservations bounded context.
/// </summary>
public interface IReservationService
{
    /// <summary>
    ///     Gets list of vehicle IDs that are booked (unavailable) during the specified period.
    /// </summary>
    /// <param name="pickupDate">Start date of the rental period.</param>
    /// <param name="returnDate">End date of the rental period.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of vehicle IDs that have confirmed or active reservations during the period.</returns>
    Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
        DateOnly pickupDate,
        DateOnly returnDate,
        CancellationToken cancellationToken = default);
}
