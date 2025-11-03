using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
/// Domain event raised when a reservation is confirmed (payment received).
/// </summary>
public sealed record ReservationConfirmed(
    ReservationIdentifier ReservationId
) : DomainEvent;
