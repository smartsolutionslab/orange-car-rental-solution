namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
///     Represents a date range for searching vehicle availability.
///     Uses DateOnly for semantic correctness (date-based searches without time components).
/// </summary>
public readonly record struct SearchPeriod
{
    private SearchPeriod(DateOnly pickupDate, DateOnly returnDate)
    {
        PickupDate = pickupDate;
        ReturnDate = returnDate;
    }

    public DateOnly PickupDate { get; }
    public DateOnly ReturnDate { get; }

    public int Days => ReturnDate.DayNumber - PickupDate.DayNumber + 1;

    /// <summary>
    ///     Creates a search period from DateOnly values.
    /// </summary>
    public static SearchPeriod Of(DateOnly pickupDate, DateOnly returnDate)
    {
        if (returnDate < pickupDate)
            throw new ArgumentException("Return date must be on or after pickup date", nameof(returnDate));

        return new SearchPeriod(pickupDate, returnDate);
    }

    /// <summary>
    ///     Creates a search period from DateTime values (for API/Application layer compatibility).
    /// </summary>
    public static SearchPeriod Of(DateTime pickupDate, DateTime returnDate) =>
        Of(DateOnly.FromDateTime(pickupDate), DateOnly.FromDateTime(returnDate));

    /// <summary>
    ///     Checks if this period overlaps with another period.
    /// </summary>
    public bool OverlapsWith(DateOnly otherPickup, DateOnly otherReturn) =>
        PickupDate <= otherReturn && ReturnDate >= otherPickup;
}
