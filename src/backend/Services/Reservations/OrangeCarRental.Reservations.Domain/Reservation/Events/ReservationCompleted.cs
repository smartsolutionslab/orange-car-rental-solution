using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

/// <summary>
///     Domain event raised when a reservation is completed (vehicle returned).
/// </summary>
public sealed record ReservationCompleted(
    ReservationIdentifier ReservationId,
    DateTime CompletedAtUtc
) : DomainEvent;
