using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

/// <summary>
///     Domain event raised when customer did not show up for pickup.
/// </summary>
public sealed record ReservationMarkedNoShow(
    ReservationIdentifier ReservationId,
    string Reason,
    DateTime MarkedAtUtc
) : DomainEvent;
