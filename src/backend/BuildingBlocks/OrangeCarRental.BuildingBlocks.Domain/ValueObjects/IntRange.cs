using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value object representing an integer range for filtering (e.g., age, seats).
/// </summary>
public sealed record IntRange(
    int? Min,
    int? Max)
{
    /// <summary>
    ///     Creates a new IntRange instance.
    /// </summary>
    /// <param name="min">Minimum value (inclusive).</param>
    /// <param name="max">Maximum value (inclusive).</param>
    /// <param name="allowNegative">Whether to allow negative values.</param>
    /// <exception cref="ArgumentException">Thrown when values are invalid.</exception>
    public static IntRange Create(int? min = null, int? max = null, bool allowNegative = false)
    {
        if (!allowNegative)
        {
            Ensure.That(min, nameof(min))
                .ThrowIf(min.HasValue && min < 0, "Minimum value cannot be negative");

            Ensure.That(max, nameof(max))
                .ThrowIf(max.HasValue && max < 0, "Maximum value cannot be negative");
        }

        Ensure.That(min, nameof(min))
            .ThrowIf(min.HasValue && max.HasValue && min > max, "Minimum value cannot be greater than maximum value");

        return new IntRange(min, max);
    }

    public static IntRange? FromNullable(int? min, int? max)
    {
        if (min is null || max is null) return null;

        return Create(min, max);
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
