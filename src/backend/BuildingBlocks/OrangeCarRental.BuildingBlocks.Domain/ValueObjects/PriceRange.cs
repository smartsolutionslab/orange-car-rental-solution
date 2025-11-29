using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value object representing a price range for filtering.
/// </summary>
public sealed record PriceRange(
    decimal? Min,
    decimal? Max)
{
    /// <summary>
    ///     Creates a new PriceRange instance.
    /// </summary>
    /// <param name="min">Minimum price (inclusive).</param>
    /// <param name="max">Maximum price (inclusive).</param>
    /// <exception cref="ArgumentException">Thrown when prices are negative or min is greater than max.</exception>
    public static PriceRange Create(decimal? min = null, decimal? max = null)
    {
        Ensure.That(min, nameof(min))
            .ThrowIf(min.HasValue && min < 0, "Minimum price cannot be negative");

        Ensure.That(max, nameof(max))
            .ThrowIf(max.HasValue && max < 0, "Maximum price cannot be negative");

        Ensure.That(min, nameof(min))
            .ThrowIf(min.HasValue && max.HasValue && min > max, "Minimum price cannot be greater than maximum price");

        return new PriceRange(min, max);
    }

    /// <summary>
    ///     Tries to create a PriceRange instance.
    /// </summary>
    public static bool TryCreate(decimal? min, decimal? max, out PriceRange? result)
    {
        if ((min.HasValue && min < 0) || (max.HasValue && max < 0) || (min.HasValue && max.HasValue && min > max))
        {
            result = null;
            return false;
        }

        result = new PriceRange(min, max);
        return true;
    }

    /// <summary>
    ///     No price range filter.
    /// </summary>
    public static PriceRange None => new(null, null);

    /// <summary>
    ///     Creates a range starting from a minimum price.
    /// </summary>
    public static PriceRange AtLeast(decimal min) => Create(min, null);

    /// <summary>
    ///     Creates a range up to a maximum price.
    /// </summary>
    public static PriceRange UpTo(decimal max) => Create(null, max);

    /// <summary>
    ///     Creates a range between two prices.
    /// </summary>
    public static PriceRange Between(decimal min, decimal max) => Create(min, max);

    /// <summary>
    ///     Creates a range for an exact price.
    /// </summary>
    public static PriceRange Exactly(decimal price) => new(price, price);

    /// <summary>
    ///     Whether any filter is specified.
    /// </summary>
    public bool HasFilter => Min.HasValue || Max.HasValue;

    /// <summary>
    ///     Whether both min and max are specified.
    /// </summary>
    public bool IsBounded => Min.HasValue && Max.HasValue;

    /// <summary>
    ///     Checks if a price falls within this range.
    /// </summary>
    public bool Contains(decimal price)
    {
        if (Min.HasValue && price < Min.Value) return false;
        if (Max.HasValue && price > Max.Value) return false;
        return true;
    }
}
