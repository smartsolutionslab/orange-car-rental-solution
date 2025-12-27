using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Reservation aggregate root (event-sourced).
///     Represents a customer's vehicle rental booking with German market-specific pricing (19% VAT).
/// </summary>
public sealed class Reservation : EventSourcedAggregate<ReservationQueryModel, ReservationIdentifier>
{
    /// <summary>
    ///     Gets the vehicle identifier.
    /// </summary>
    public VehicleIdentifier? VehicleIdentifier => State.VehicleIdentifier;

    /// <summary>
    ///     Gets the customer identifier.
    /// </summary>
    public CustomerIdentifier? CustomerIdentifier => State.CustomerIdentifier;

    /// <summary>
    ///     Gets the booking period.
    /// </summary>
    public BookingPeriod? Period => State.Period;

    /// <summary>
    ///     Gets the pickup location code.
    /// </summary>
    public LocationCode? PickupLocationCode => State.PickupLocationCode;

    /// <summary>
    ///     Gets the dropoff location code.
    /// </summary>
    public LocationCode? DropoffLocationCode => State.DropoffLocationCode;

    /// <summary>
    ///     Gets the total price.
    /// </summary>
    public Money? TotalPrice => State.TotalPrice;

    /// <summary>
    ///     Gets the reservation status.
    /// </summary>
    public ReservationStatus Status => State.Status;

    /// <summary>
    ///     Gets the cancellation reason (if cancelled).
    /// </summary>
    public string? CancellationReason => State.CancellationReason;

    /// <summary>
    ///     Gets when the reservation was created.
    /// </summary>
    public DateTime CreatedAt => State.CreatedAtUtc;

    /// <summary>
    ///     Gets when the reservation was confirmed.
    /// </summary>
    public DateTime? ConfirmedAt => State.ConfirmedAtUtc;

    /// <summary>
    ///     Gets when the reservation was cancelled.
    /// </summary>
    public DateTime? CancelledAt => State.CancelledAtUtc;

    /// <summary>
    ///     Gets when the reservation was completed.
    /// </summary>
    public DateTime? CompletedAt => State.CompletedAtUtc;

    /// <summary>
    ///     Create a new pending reservation.
    /// </summary>
    public void Create(
        VehicleIdentifier vehicleIdentifier,
        CustomerIdentifier customerIdentifier,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice)
    {
        EnsureDoesNotExist();

        var now = DateTime.UtcNow;
        Apply(new ReservationCreated(
            ReservationIdentifier.New(),
            vehicleIdentifier,
            customerIdentifier,
            period,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice,
            now));
    }

    /// <summary>
    ///     Confirm the reservation (payment received).
    /// </summary>
    public void Confirm()
    {
        EnsureExists();

        if (State.Status != ReservationStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm reservation in status: {State.Status}");

        var now = DateTime.UtcNow;
        Apply(new ReservationConfirmed(Id, now));
    }

    /// <summary>
    ///     Cancel the reservation.
    /// </summary>
    public void Cancel(string? reason = null)
    {
        EnsureExists();

        if (State.Status == ReservationStatus.Cancelled)
            return; // Already cancelled, idempotent

        if (State.Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed reservation");

        if (State.Status == ReservationStatus.Active)
            throw new InvalidOperationException("Cannot cancel an active rental. Please return the vehicle first.");

        var now = DateTime.UtcNow;
        Apply(new ReservationCancelled(Id, reason, now));
    }

    /// <summary>
    ///     Mark reservation as active (vehicle picked up).
    /// </summary>
    public void MarkAsActive()
    {
        EnsureExists();

        if (State.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException($"Cannot activate reservation in status: {State.Status}");

        if (!State.Period.HasValue)
            throw new InvalidOperationException("Reservation has no booking period");

        // Check if pickup date is today or in the past
        if (State.Period.Value.PickupDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new InvalidOperationException("Cannot activate reservation before pickup date");

        var now = DateTime.UtcNow;
        Apply(new ReservationActivated(Id, now));
    }

    /// <summary>
    ///     Complete the reservation (vehicle returned).
    /// </summary>
    public void Complete()
    {
        EnsureExists();

        if (State.Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Cannot complete reservation in status: {State.Status}");

        var now = DateTime.UtcNow;
        Apply(new ReservationCompleted(Id, now));
    }

    /// <summary>
    ///     Mark as no-show if customer didn't pick up vehicle.
    /// </summary>
    public void MarkAsNoShow()
    {
        EnsureExists();

        if (State.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException($"Cannot mark as no-show in status: {State.Status}");

        if (!State.Period.HasValue)
            throw new InvalidOperationException("Reservation has no booking period");

        // Check if we're past the pickup date
        if (DateOnly.FromDateTime(DateTime.UtcNow) <= State.Period.Value.PickupDate)
            throw new InvalidOperationException("Cannot mark as no-show before pickup date has passed");

        var now = DateTime.UtcNow;
        Apply(new ReservationMarkedNoShow(Id, "Customer did not show up for pickup", now));
    }

    /// <summary>
    ///     Check if the reservation period overlaps with another period.
    /// </summary>
    public bool OverlapsWith(BookingPeriod otherPeriod)
    {
        if (!State.Period.HasValue)
            return false;

        return State.Period.Value.OverlapsWith(otherPeriod);
    }
}
