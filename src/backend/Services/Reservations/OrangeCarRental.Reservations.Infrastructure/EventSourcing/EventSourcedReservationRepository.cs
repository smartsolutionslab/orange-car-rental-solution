using Eventuous;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.EventSourcing;

/// <summary>
/// Event-sourced repository for Reservation aggregates.
/// Uses Eventuous IEventReader/IEventWriter for event persistence and aggregate rehydration.
/// </summary>
public sealed class EventSourcedReservationRepository(IEventReader eventReader, IEventWriter eventWriter)
    : IEventSourcedReservationRepository
{
    private const string StreamPrefix = "Reservation-";

    /// <summary>
    /// Loads a Reservation aggregate from the event store by replaying its events.
    /// </summary>
    public async Task<Reservation> LoadAsync(
        ReservationIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var streamName = GetStreamName(id);
        var reservation = new Reservation();

        // Read events from the stream and fold them into the aggregate
        var events = await eventReader.ReadStream(streamName, StreamReadPosition.Start, failIfNotFound: false, cancellationToken);
        reservation.Load(events.Select(e => e.Payload).ToList());

        return reservation;
    }

    /// <summary>
    /// Saves a Reservation aggregate's uncommitted events to the event store.
    /// </summary>
    public async Task SaveAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        if (reservation.Changes.Count == 0)
            return;

        var streamName = GetStreamName(reservation.Id);
        // For new aggregates, use ExpectedStreamVersion.NoStream
        // For existing aggregates, calculate from the current version
        var currentVersion = reservation.CurrentVersion;
        var expectedVersion = currentVersion == -1
            ? ExpectedStreamVersion.NoStream
            : new ExpectedStreamVersion(currentVersion - reservation.Changes.Count);

        await eventWriter.Store(
            streamName,
            expectedVersion,
            reservation.Changes.ToList(),
            null,
            cancellationToken);

        reservation.ClearChanges();
    }

    /// <summary>
    /// Checks if a reservation exists in the event store.
    /// </summary>
    public async Task<bool> ExistsAsync(
        ReservationIdentifier id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reservation = await LoadAsync(id, cancellationToken);
            return reservation.State.HasBeenCreated;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static StreamName GetStreamName(ReservationIdentifier id) =>
        new($"{StreamPrefix}{id.Value}");
}

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
