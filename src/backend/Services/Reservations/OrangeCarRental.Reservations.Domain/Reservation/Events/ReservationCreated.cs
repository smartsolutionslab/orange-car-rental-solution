using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

/// <summary>
///     Domain event raised when a new reservation is created.
///     Contains all data needed to reconstruct the reservation state.
/// </summary>
public sealed record ReservationCreated(
    ReservationIdentifier ReservationId,
    VehicleIdentifier VehicleIdentifier,
    CustomerIdentifier CustomerIdentifier,
    BookingPeriod Period,
    LocationCode PickupLocationCode,
    LocationCode DropoffLocationCode,
    Money TotalPrice,
    DateTime CreatedAtUtc
) : DomainEvent;
