using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Reservation aggregate root.
///     Represents a customer's vehicle rental booking with German market-specific pricing (19% VAT).
/// </summary>
public sealed class Reservation : AggregateRoot<ReservationIdentifier>
{
    // For EF Core
    private Reservation()
    {
        VehicleIdentifier = default;
        CustomerIdentifier = default;
        Period = default!;
        PickupLocationCode = default;
        DropoffLocationCode = default;
        TotalPrice = default;
    }

    private Reservation(
        ReservationIdentifier id,
        VehicleIdentifier vehicleIdentifier,
        CustomerIdentifier customerIdentifier,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice)
        : base(id)
    {
        VehicleIdentifier = vehicleIdentifier;
        CustomerIdentifier = customerIdentifier;
        Period = period;
        PickupLocationCode = pickupLocationCode;
        DropoffLocationCode = dropoffLocationCode;
        TotalPrice = totalPrice;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationCreated(
            Id,
            VehicleIdentifier,
            CustomerIdentifier,
            Period,
            TotalPrice));
    }

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public VehicleIdentifier VehicleIdentifier { get; init; }
    public CustomerIdentifier CustomerIdentifier { get; init; }
    public BookingPeriod Period { get; init; }
    public LocationCode PickupLocationCode { get; init; }
    public LocationCode DropoffLocationCode { get; init; }
    public Money TotalPrice { get; init; }
    public ReservationStatus Status { get; init; }
    public string? CancellationReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public DateTime? CompletedAt { get; init; }

    /// <summary>
    ///     Create a new pending reservation.
    /// </summary>
    public static Reservation Create(
        VehicleIdentifier vehicleIdentifier,
        CustomerIdentifier customerIdentifier,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice)
    {
        // Value objects already validate on creation, no need for additional checks here

        return new Reservation(
            ReservationIdentifier.New(),
            vehicleIdentifier,
            customerIdentifier,
            period,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice
        );
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    ///     Does not raise domain events - caller is responsible for that.
    /// </summary>
    private Reservation CreateMutatedCopy(
        VehicleIdentifier? vehicleId = null,
        CustomerIdentifier? customerId = null,
        BookingPeriod? period = null,
        LocationCode? pickupLocationCode = null,
        LocationCode? dropoffLocationCode = null,
        Money? totalPrice = null,
        ReservationStatus? status = null,
        string? cancellationReason = null,
        DateTime? createdAt = null,
        DateTime? confirmedAt = null,
        DateTime? cancelledAt = null,
        DateTime? completedAt = null)
    {
        return new Reservation
        {
            Id = Id,
            VehicleIdentifier = vehicleId ?? VehicleIdentifier,
            CustomerIdentifier = customerId ?? CustomerIdentifier,
            Period = period ?? Period,
            PickupLocationCode = pickupLocationCode ?? PickupLocationCode,
            DropoffLocationCode = dropoffLocationCode ?? DropoffLocationCode,
            TotalPrice = totalPrice ?? TotalPrice,
            Status = status ?? Status,
            CancellationReason = cancellationReason ?? CancellationReason,
            CreatedAt = createdAt ?? CreatedAt,
            ConfirmedAt = confirmedAt ?? ConfirmedAt,
            CancelledAt = cancelledAt ?? CancelledAt,
            CompletedAt = completedAt ?? CompletedAt
        };
    }

    /// <summary>
    ///     Confirm the reservation (payment received).
    ///     Returns a new instance with confirmed status (immutable pattern).
    /// </summary>
    public Reservation Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm reservation in status: {Status}");

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            status: ReservationStatus.Confirmed,
            confirmedAt: now);

        updated.AddDomainEvent(new ReservationConfirmed(Id));

        return updated;
    }

    /// <summary>
    ///     Cancel the reservation.
    ///     Returns a new instance with cancelled status (immutable pattern).
    /// </summary>
    public Reservation Cancel(string? reason = null)
    {
        if (Status == ReservationStatus.Cancelled) return this; // Already cancelled
        if (Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed reservation");
        if (Status == ReservationStatus.Active)
            throw new InvalidOperationException("Cannot cancel an active rental. Please return the vehicle first.");

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            status: ReservationStatus.Cancelled,
            cancellationReason: reason,
            cancelledAt: now);

        updated.AddDomainEvent(new ReservationCancelled(Id, reason));

        return updated;
    }

    /// <summary>
    ///     Mark reservation as active (vehicle picked up).
    ///     Returns a new instance with active status (immutable pattern).
    /// </summary>
    public Reservation MarkAsActive()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException($"Cannot activate reservation in status: {Status}");

        // Check if pickup date is today or in the past
        if (Period.PickupDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new InvalidOperationException("Cannot activate reservation before pickup date");

        return CreateMutatedCopy(status: ReservationStatus.Active);
    }

    /// <summary>
    ///     Complete the reservation (vehicle returned).
    ///     Returns a new instance with completed status (immutable pattern).
    /// </summary>
    public Reservation Complete()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Cannot complete reservation in status: {Status}");

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            status: ReservationStatus.Completed,
            completedAt: now);

        updated.AddDomainEvent(new ReservationCompleted(Id));

        return updated;
    }

    /// <summary>
    ///     Mark as no-show if customer didn't pick up vehicle.
    ///     Returns a new instance with no-show status (immutable pattern).
    /// </summary>
    public Reservation MarkAsNoShow()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException($"Cannot mark as no-show in status: {Status}");

        // Check if we're past the pickup date
        if (DateOnly.FromDateTime(DateTime.UtcNow) <= Period.PickupDate)
            throw new InvalidOperationException("Cannot mark as no-show before pickup date has passed");

        var now = DateTime.UtcNow;
        return CreateMutatedCopy(
            status: ReservationStatus.NoShow,
            cancelledAt: now,
            cancellationReason: "Customer did not show up for pickup");
    }

    /// <summary>
    ///     Check if the reservation period overlaps with another period.
    /// </summary>
    public bool OverlapsWith(BookingPeriod otherPeriod) => Period.OverlapsWith(otherPeriod);
}
