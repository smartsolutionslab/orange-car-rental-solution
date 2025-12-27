using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.EventSourcing;

/// <summary>
/// Interface for publishing domain events from Reservation aggregate.
/// Implementations can publish to message buses, event stores, etc.
/// </summary>
public interface IReservationEventPublisher
{
    /// <summary>
    /// Publishes all uncommitted events from the reservation aggregate.
    /// </summary>
    Task PublishEventsAsync(Reservation reservation, CancellationToken cancellationToken = default);
}

/// <summary>
/// No-op event publisher for when event publishing is not configured.
/// Can be replaced with implementations for Azure Service Bus, RabbitMQ, etc.
/// </summary>
public class NullReservationEventPublisher : IReservationEventPublisher
{
    public Task PublishEventsAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        // Events are available via reservation.Changes but not published anywhere
        // Replace with actual implementation when message bus is configured
        return Task.CompletedTask;
    }
}
