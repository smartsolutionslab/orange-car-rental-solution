namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value object representing an integer range for filtering (e.g., age, seats).
/// </summary>
public sealed record IntRange
{
    /// <summary>
    ///     The minimum value (inclusive).
    /// </summary>
    public int? Min { get; }

    /// <summary>
    ///     The maximum value (inclusive).
    /// </summary>
    public int? Max { get; }

    private IntRange(int? min, int? max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    ///     Creates a new IntRange instance.
    /// </summary>
    /// <param name="min">Minimum value (inclusive).</param>
    /// <param name="max">Maximum value (inclusive).</param>
    /// <param name="allowNegative">Whether to allow negative values.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when values are invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when min is greater than max.</exception>
    public static IntRange Create(int? min = null, int? max = null, bool allowNegative = false)
    {
        if (!allowNegative)
        {
            if (min.HasValue && min < 0)
                throw new ArgumentOutOfRangeException(nameof(min), "Minimum value cannot be negative");

            if (max.HasValue && max < 0)
                throw new ArgumentOutOfRangeException(nameof(max), "Maximum value cannot be negative");
        }

        if (min.HasValue && max.HasValue && min > max)
            throw new ArgumentException("Minimum value cannot be greater than maximum value");

        return new IntRange(min, max);
    }

    /// <summary>
    ///     Tries to create an IntRange instance.
    /// </summary>
    public static bool TryCreate(int? min, int? max, out IntRange? result, bool allowNegative = false)
    {
        if (!allowNegative && ((min.HasValue && min < 0) || (max.HasValue && max < 0)))
        {
            result = null;
            return false;
        }

        if (min.HasValue && max.HasValue && min > max)
        {
            result = null;
            return false;
        }

        result = new IntRange(min, max);
        return true;
    }

    /// <summary>
    ///     No range filter.
    /// </summary>
    public static IntRange None => new(null, null);

    /// <summary>
    ///     Creates a range starting from a minimum value.
    /// </summary>
    public static IntRange AtLeast(int min) => Create(min, null);

    /// <summary>
    ///     Creates a range up to a maximum value.
    /// </summary>
    public static IntRange UpTo(int max) => Create(null, max);

    /// <summary>
    ///     Creates a range between two values.
    /// </summary>
    public static IntRange Between(int min, int max) => Create(min, max);

    /// <summary>
    ///     Creates a range for an exact value.
    /// </summary>
    public static IntRange Exactly(int value) => new(value, value);

    /// <summary>
    ///     Whether any filter is specified.
    /// </summary>
    public bool HasFilter => Min.HasValue || Max.HasValue;

    /// <summary>
    ///     Whether both min and max are specified.
    /// </summary>
    public bool IsBounded => Min.HasValue && Max.HasValue;

    /// <summary>
    ///     Checks if a value falls within this range.
    /// </summary>
    public bool Contains(int value)
    {
        if (Min.HasValue && value < Min.Value) return false;
        if (Max.HasValue && value > Max.Value) return false;
        return true;
    }
}
