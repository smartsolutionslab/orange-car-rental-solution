namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Represents the rental period with pickup and return dates.
///     Validates that the period is valid and meets minimum/maximum rental duration requirements.
/// </summary>
public readonly record struct BookingPeriod(DateOnly PickupDate, DateOnly ReturnDate)
{
    /// <summary>
    ///     Number of rental days (minimum 1 day).
    /// </summary>
    public int Days => ReturnDate.DayNumber - PickupDate.DayNumber + 1;

    public static BookingPeriod Of(DateOnly pickupDate, DateOnly returnDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (pickupDate < today) throw new ArgumentException("Pickup date cannot be in the past", nameof(pickupDate));
        if (returnDate <= pickupDate)
            throw new ArgumentException("Return date must be after pickup date", nameof(returnDate));

        var days = returnDate.DayNumber - pickupDate.DayNumber + 1;
        if (days > 90) throw new ArgumentException("Rental period cannot exceed 90 days", nameof(returnDate));

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
