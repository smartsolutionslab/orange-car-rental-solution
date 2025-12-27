using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

/// <summary>
///     Domain event raised when a reservation becomes active (vehicle picked up).
/// </summary>
public sealed record ReservationActivated(
    ReservationIdentifier ReservationId,
    DateTime ActivatedAtUtc
) : DomainEvent;
