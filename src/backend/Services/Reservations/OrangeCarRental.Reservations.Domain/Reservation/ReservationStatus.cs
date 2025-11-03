namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
/// Represents the current status of a reservation in the booking lifecycle.
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation has been created but not yet confirmed (awaiting payment).
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Reservation has been confirmed and payment received.
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Customer has picked up the vehicle and rental is active.
    /// </summary>
    Active = 2,

    /// <summary>
    /// Rental completed, vehicle returned.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Reservation was cancelled before pickup.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Customer did not show up for pickup (no-show).
    /// </summary>
    NoShow = 5
}
