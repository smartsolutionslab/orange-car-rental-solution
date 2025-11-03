using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

/// <summary>
/// Domain event raised when a new reservation is created.
/// </summary>
public sealed record ReservationCreated(
    ReservationIdentifier ReservationId,
    Guid VehicleId,
    Guid CustomerId,
    BookingPeriod Period,
    Money TotalPrice
) : DomainEvent;
