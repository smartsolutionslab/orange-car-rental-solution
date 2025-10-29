using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Events;

/// <summary>
/// Domain event raised when a reservation is confirmed (payment received).
/// </summary>
public sealed record ReservationConfirmed(
    ReservationIdentifier ReservationId
) : DomainEvent;
