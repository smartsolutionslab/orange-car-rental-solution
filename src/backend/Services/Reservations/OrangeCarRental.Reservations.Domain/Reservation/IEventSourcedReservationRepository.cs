namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
/// Repository interface for event-sourced Reservation aggregate operations.
/// Separates event store operations from read model queries.
/// </summary>
public interface IEventSourcedReservationRepository
{
    /// <summary>
    /// Loads a Reservation aggregate from the event store.
    /// </summary>
    Task<Reservation> LoadAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a Reservation aggregate's events to the event store.
    /// </summary>
    Task SaveAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a reservation exists in the event store.
    /// </summary>
    Task<bool> ExistsAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);
}
