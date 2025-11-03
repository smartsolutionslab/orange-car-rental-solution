using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
/// Reservation aggregate root.
/// Represents a customer's vehicle rental booking with German market-specific pricing (19% VAT).
/// </summary>
public sealed class Reservation : AggregateRoot<ReservationIdentifier>
{
    public Guid VehicleId { get; private set; }
    public Guid CustomerId { get; private set; }
    public BookingPeriod Period { get; private set; }
    public LocationCode PickupLocationCode { get; private set; }
    public LocationCode DropoffLocationCode { get; private set; }
    public Money TotalPrice { get; private set; }
    public ReservationStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // For EF Core
    private Reservation()
    {
        Period = default!;
        PickupLocationCode = default;
        DropoffLocationCode = default;
        TotalPrice = default;
    }

    private Reservation(
        ReservationIdentifier id,
        Guid vehicleId,
        Guid customerId,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice)
        : base(id)
    {
        VehicleId = vehicleId;
        CustomerId = customerId;
        Period = period;
        PickupLocationCode = pickupLocationCode;
        DropoffLocationCode = dropoffLocationCode;
        TotalPrice = totalPrice;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationCreated(Id, VehicleId, CustomerId, Period, TotalPrice));
    }

    /// <summary>
    /// Create a new pending reservation.
    /// </summary>
    public static Reservation Create(
        Guid vehicleId,
        Guid customerId,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice)
    {
        if (vehicleId == Guid.Empty) throw new ArgumentException("Vehicle ID cannot be empty", nameof(vehicleId));

        if (customerId == Guid.Empty) throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        return new Reservation(
            ReservationIdentifier.New(),
            vehicleId,
            customerId,
            period,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice
        );
    }

    /// <summary>
    /// Confirm the reservation (payment received).
    /// </summary>
    public void Confirm()
    {
        if (Status != ReservationStatus.Pending) throw new InvalidOperationException($"Cannot confirm reservation in status: {Status}");

        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationConfirmed(Id));
    }

    /// <summary>
    /// Cancel the reservation.
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (Status == ReservationStatus.Cancelled) return; // Already cancelled

        if (Status == ReservationStatus.Completed) throw new InvalidOperationException("Cannot cancel a completed reservation");

        if (Status == ReservationStatus.Active) throw new InvalidOperationException("Cannot cancel an active rental. Please return the vehicle first.");

        Status = ReservationStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationCancelled(Id, reason));
    }

    /// <summary>
    /// Mark reservation as active (vehicle picked up).
    /// </summary>
    public void MarkAsActive()
    {
        if (Status != ReservationStatus.Confirmed) throw new InvalidOperationException($"Cannot activate reservation in status: {Status}");


        // Check if pickup date is today or in the past
        if (Period.PickupDate > DateTime.UtcNow.Date) throw new InvalidOperationException("Cannot activate reservation before pickup date");

        Status = ReservationStatus.Active;
    }

    /// <summary>
    /// Complete the reservation (vehicle returned).
    /// </summary>
    public void Complete()
    {
        if (Status != ReservationStatus.Active) throw new InvalidOperationException($"Cannot complete reservation in status: {Status}");

        Status = ReservationStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationCompleted(Id));
    }

    /// <summary>
    /// Mark as no-show if customer didn't pick up vehicle.
    /// </summary>
    public void MarkAsNoShow()
    {
        if (Status != ReservationStatus.Confirmed) throw new InvalidOperationException($"Cannot mark as no-show in status: {Status}");

        // Check if we're past the pickup date
        if (DateTime.UtcNow.Date <= Period.PickupDate) throw new InvalidOperationException("Cannot mark as no-show before pickup date has passed");

        Status = ReservationStatus.NoShow;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = "Customer did not show up for pickup";
    }

    /// <summary>
    /// Check if the reservation period overlaps with another period.
    /// </summary>
    public bool OverlapsWith(BookingPeriod otherPeriod) => Period.OverlapsWith(otherPeriod);
}
