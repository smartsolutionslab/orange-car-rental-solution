namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

/// <summary>
/// Represents the rental period with pickup and return dates.
/// Validates that the period is valid and meets minimum/maximum rental duration requirements.
/// </summary>
public readonly record struct BookingPeriod
{
    public DateTime PickupDate { get; }
    public DateTime ReturnDate { get; }

    /// <summary>
    /// Number of rental days (minimum 1 day).
    /// </summary>
    public int Days => (ReturnDate.Date - PickupDate.Date).Days + 1;

    private BookingPeriod(DateTime pickupDate, DateTime returnDate)
    {
        PickupDate = pickupDate;
        ReturnDate = returnDate;
    }

    public static BookingPeriod Of(DateTime pickupDate, DateTime returnDate)
    {
        // Normalize to dates only (remove time component)
        var pickup = pickupDate.Date;
        var returnDt = returnDate.Date;

        // Validation: pickup date must not be in the past
        if (pickup < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Pickup date cannot be in the past", nameof(pickupDate));
        }

        // Validation: return date must be after pickup date
        if (returnDt <= pickup)
        {
            throw new ArgumentException("Return date must be after pickup date", nameof(returnDate));
        }

        // Validation: maximum rental period (e.g., 90 days)
        var days = (returnDt - pickup).Days + 1;
        if (days > 90)
        {
            throw new ArgumentException("Rental period cannot exceed 90 days", nameof(returnDate));
        }

        return new BookingPeriod(pickup, returnDt);
    }

    /// <summary>
    /// Checks if this booking period overlaps with another period.
    /// </summary>
    public bool OverlapsWith(BookingPeriod other)
    {
        return PickupDate <= other.ReturnDate && ReturnDate >= other.PickupDate;
    }

    public override string ToString() => $"{PickupDate:yyyy-MM-dd} to {ReturnDate:yyyy-MM-dd} ({Days} days)";
}
