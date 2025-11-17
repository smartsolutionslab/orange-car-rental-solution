using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

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
    /// <param name="period">Start and end date of the rental period.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of vehicle IDs that have confirmed or active reservations during the period.</returns>
    Task<IReadOnlyList<VehicleIdentifier>> GetBookedVehicleIdsAsync(SearchPeriod period, CancellationToken cancellationToken = default);
}
