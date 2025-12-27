using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

/// <summary>
/// Service for executing commands on Reservation aggregates using event sourcing.
/// Provides methods to load, mutate, and persist reservation aggregates through the event store.
/// </summary>
public interface IReservationCommandService
{
    /// <summary>
    /// Creates a new pending reservation.
    /// </summary>
    Task<Reservation> CreateAsync(
        VehicleIdentifier vehicleId,
        CustomerIdentifier customerId,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a pending reservation (payment received).
    /// </summary>
    Task<Reservation> ConfirmAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a reservation.
    /// </summary>
    Task<Reservation> CancelAsync(
        ReservationIdentifier reservationId,
        string? reason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a reservation as active (vehicle picked up).
    /// </summary>
    Task<Reservation> ActivateAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a reservation (vehicle returned).
    /// </summary>
    Task<Reservation> CompleteAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a reservation as no-show.
    /// </summary>
    Task<Reservation> MarkAsNoShowAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default);
}
