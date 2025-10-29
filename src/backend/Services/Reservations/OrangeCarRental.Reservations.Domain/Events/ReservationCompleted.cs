using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Events;

/// <summary>
/// Domain event raised when a reservation is completed (vehicle returned).
/// </summary>
public sealed record ReservationCompleted(
    ReservationIdentifier ReservationId
) : DomainEvent;
