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

    Task<List<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(List<Reservation> Reservations, int TotalCount)> SearchAsync(
        ReservationStatus? status = null,
        Guid? customerId = null,
        string? customerName = null,
        Guid? vehicleId = null,
        string? categoryCode = null,
        string? pickupLocationCode = null,
        DateOnly? pickupDateFrom = null,
        DateOnly? pickupDateTo = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        string? sortBy = null,
        bool sortDescending = false,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task DeleteAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets list of vehicle IDs that are booked (unavailable) during the specified period.
    ///     Used to determine vehicle availability for new reservations.
    /// </summary>
    /// <param name="period">Start date and end date of the period.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of vehicle IDs that have active or confirmed reservations during the period.</returns>
    Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
        BookingPeriod period,
        CancellationToken cancellationToken = default);
}
