using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Events;

/// <summary>
/// Domain event raised when a reservation is cancelled.
/// </summary>
public sealed record ReservationCancelled(
    ReservationIdentifier ReservationId,
    string? CancellationReason
) : DomainEvent;
