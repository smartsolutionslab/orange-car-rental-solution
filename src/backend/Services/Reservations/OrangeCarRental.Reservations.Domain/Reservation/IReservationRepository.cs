using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Repository interface for Reservation aggregate.
/// </summary>
public interface IReservationRepository
{
    /// <summary>
    ///     Gets a reservation by its identifier.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reservation.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when the reservation is not found.</exception>
    Task<Reservation> GetByIdAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all reservations (use with caution - prefer SearchAsync for large datasets).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An immutable read-only list of all reservations.</returns>
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all reservations. Memory-efficient alternative to GetAllAsync.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of all reservations.</returns>
    IAsyncEnumerable<Reservation> StreamAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Searches reservations with filters, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Search parameters including filters, sorting, and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged result containing reservations and pagination metadata.</returns>
    Task<PagedResult<Reservation>> SearchAsync(
        ReservationSearchParameters parameters,
        CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task DeleteAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets list of vehicle IDs that are booked (unavailable) during the specified period.
    ///     Used to determine vehicle availability for new reservations.
    /// </summary>
    /// <param name="period">Start date and end date of the period.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of vehicle IDs that have active or confirmed reservations during the period.</returns>
    Task<IReadOnlyList<VehicleIdentifier>> GetBookedVehicleIdsAsync(
        BookingPeriod period,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
