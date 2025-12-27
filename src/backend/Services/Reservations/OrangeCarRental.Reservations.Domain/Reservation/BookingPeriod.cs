using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Represents the rental period with pickup and return dates.
///     Validates that the period is valid and meets minimum/maximum rental duration requirements.
/// </summary>
public readonly record struct BookingPeriod(DateOnly PickupDate, DateOnly ReturnDate) : IValueObject
{
    /// <summary>
    ///     Number of rental days (minimum 1 day).
    /// </summary>
    public int Days => ReturnDate.DayNumber - PickupDate.DayNumber + 1;

    public static BookingPeriod Of(DateOnly pickupDate, DateOnly returnDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        Ensure.That(pickupDate, nameof(pickupDate))
            .ThrowIf(pickupDate < today, "Pickup date cannot be in the past");

        Ensure.That(returnDate, nameof(returnDate))
            .ThrowIf(returnDate <= pickupDate, "Return date must be after pickup date");

        var days = returnDate.DayNumber - pickupDate.DayNumber + 1;
        Ensure.That(returnDate, nameof(returnDate))
            .ThrowIf(days > 90, "Rental period cannot exceed 90 days");

        return new BookingPeriod(pickupDate, returnDate);
    }

    /// <summary>
    ///     Factory method that accepts DateTime and converts to DateOnly.
    ///     Useful for API/DTO conversions.
    /// </summary>
    public static BookingPeriod Of(DateTime pickupDate, DateTime returnDate) =>
        Of(DateOnly.FromDateTime(pickupDate), DateOnly.FromDateTime(returnDate));

    /// <summary>
    ///     Checks if this booking period overlaps with another period.
    /// </summary>
    public bool OverlapsWith(BookingPeriod other) =>
        PickupDate <= other.ReturnDate && ReturnDate >= other.PickupDate;

    public override string ToString() =>
        $"{PickupDate:yyyy-MM-dd} to {ReturnDate:yyyy-MM-dd} ({Days} days)";
}
