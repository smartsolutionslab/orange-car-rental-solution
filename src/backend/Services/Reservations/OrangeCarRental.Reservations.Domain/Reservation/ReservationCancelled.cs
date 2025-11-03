using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
/// Domain event raised when a reservation is cancelled.
/// </summary>
public sealed record ReservationCancelled(
    ReservationIdentifier ReservationId,
    string? CancellationReason
) : DomainEvent;
