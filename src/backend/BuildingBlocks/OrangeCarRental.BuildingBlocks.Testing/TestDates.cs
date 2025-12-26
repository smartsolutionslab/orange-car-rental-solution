namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
/// Helper methods for creating test dates.
/// All dates are based on UTC to ensure consistent test behavior.
/// </summary>
public static class TestDates
{
    /// <summary>
    /// Today's date (UTC).
    /// </summary>
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Tomorrow's date.
    /// </summary>
    public static DateOnly Tomorrow => Today.AddDays(1);

    /// <summary>
    /// Yesterday's date.
    /// </summary>
    public static DateOnly Yesterday => Today.AddDays(-1);

    /// <summary>
    /// Gets a date N days in the future.
    /// </summary>
    public static DateOnly FutureDays(int days) => Today.AddDays(days);

    /// <summary>
    /// Gets a date N days in the past.
    /// </summary>
    public static DateOnly PastDays(int days) => Today.AddDays(-days);

    /// <summary>
    /// Default pickup date for tests (7 days from now).
    /// </summary>
    public static DateOnly DefaultPickup => FutureDays(7);

    /// <summary>
    /// Default return date for tests (10 days from now, 3-day rental).
    /// </summary>
    public static DateOnly DefaultReturn => FutureDays(10);

    /// <summary>
    /// Creates a pickup/return date pair for a rental.
    /// </summary>
    /// <param name="startDaysFromNow">Days from now for pickup.</param>
    /// <param name="rentalDays">Number of rental days.</param>
    public static (DateOnly Pickup, DateOnly Return) RentalPeriod(
        int startDaysFromNow = 7,
        int rentalDays = 3)
    {
        var pickup = FutureDays(startDaysFromNow);
        var returnDate = pickup.AddDays(rentalDays);
        return (pickup, returnDate);
    }

    /// <summary>
    /// A birthdate that makes the person 30 years old.
    /// </summary>
    public static DateOnly Adult30 => Today.AddYears(-30);

    /// <summary>
    /// A birthdate that makes the person 25 years old.
    /// </summary>
    public static DateOnly Adult25 => Today.AddYears(-25);

    /// <summary>
    /// A birthdate that makes the person 21 years old (minimum rental age in Germany).
    /// </summary>
    public static DateOnly Adult21 => Today.AddYears(-21);

    /// <summary>
    /// A birthdate that makes the person 18 years old.
    /// </summary>
    public static DateOnly Adult18 => Today.AddYears(-18);

    /// <summary>
    /// A driver's license issue date (5 years ago).
    /// </summary>
    public static DateOnly LicenseIssued5YearsAgo => Today.AddYears(-5);

    /// <summary>
    /// A driver's license expiry date (5 years from now).
    /// </summary>
    public static DateOnly LicenseExpiry5Years => Today.AddYears(5);
}
