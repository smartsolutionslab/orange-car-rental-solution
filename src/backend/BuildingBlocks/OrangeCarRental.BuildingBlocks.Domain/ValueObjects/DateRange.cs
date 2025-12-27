using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value object representing a date range for filtering.
/// </summary>
public sealed record DateRange
{
    /// <summary>
    ///     The start date (inclusive).
    /// </summary>
    public DateOnly? From { get; }

    /// <summary>
    ///     The end date (inclusive).
    /// </summary>
    public DateOnly? To { get; }

    private DateRange(DateOnly? from, DateOnly? to)
    {
        From = from;
        To = to;
    }

    /// <summary>
    ///     Creates a new DateRange instance.
    /// </summary>
    /// <param name="from">Start date (inclusive).</param>
    /// <param name="to">End date (inclusive).</param>
    /// <exception cref="ArgumentException">Thrown when from is after to.</exception>
    public static DateRange Create(DateOnly? from = null, DateOnly? to = null)
    {
        Ensure.That(from, nameof(from))
            .ThrowIf(from.HasValue && to.HasValue && from > to, "From date cannot be after To date");

        return new DateRange(from, to);
    }

    /// <summary>
    ///     Tries to create a DateRange instance.
    /// </summary>
    public static bool TryCreate(DateOnly? from, DateOnly? to, out DateRange? result)
    {
        if (from.HasValue && to.HasValue && from > to)
        {
            result = null;
            return false;
        }

        result = new DateRange(from, to);
        return true;
    }

    /// <summary>
    ///     No date range filter.
    /// </summary>
    public static DateRange None => new(null, null);

    /// <summary>
    ///     Creates a range starting from a specific date.
    /// </summary>
    public static DateRange StartingFrom(DateOnly from) => new(from, null);

    /// <summary>
    ///     Creates a range ending at a specific date.
    /// </summary>
    public static DateRange EndingAt(DateOnly to) => new(null, to);

    /// <summary>
    ///     Creates a range between two dates.
    /// </summary>
    public static DateRange Between(DateOnly from, DateOnly to) => Create(from, to);

    /// <summary>
    ///     Creates a range for a single day.
    /// </summary>
    public static DateRange SingleDay(DateOnly date) => new(date, date);

    /// <summary>
    ///     Whether any filter is specified.
    /// </summary>
    public bool HasFilter => From.HasValue || To.HasValue;

    /// <summary>
    ///     Whether both from and to are specified.
    /// </summary>
    public bool IsBounded => From.HasValue && To.HasValue;

    /// <summary>
    ///     Checks if a date falls within this range.
    /// </summary>
    public bool Contains(DateOnly date)
    {
        if (From.HasValue && date < From.Value) return false;
        if (To.HasValue && date > To.Value) return false;
        return true;
    }

    /// <summary>
    ///     Gets the number of days in the range (if bounded).
    /// </summary>
    public int? DaysInRange => IsBounded ? To!.Value.DayNumber - From!.Value.DayNumber + 1 : null;
}
