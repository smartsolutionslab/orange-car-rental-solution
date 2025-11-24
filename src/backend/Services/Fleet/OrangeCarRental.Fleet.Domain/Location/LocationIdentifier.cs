using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Strongly-typed identifier for Location aggregate.
///     Uses GUID v7 for time-ordered identifiers with better database performance.
/// </summary>
public readonly record struct LocationIdentifier(Guid Value) : IValueObject
{
    /// <summary>
    ///     Creates a new unique location identifier using GUID v7.
    /// </summary>
    public static LocationIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a location identifier from an existing GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the GUID is empty.</exception>
    public static LocationIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new LocationIdentifier(value);
    }

    /// <summary>
    ///     Creates a location identifier from a string representation of a GUID.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid GUID.</exception>
    public static LocationIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"Invalid location ID format: {value}", nameof(value));

        return From(guid);
    }

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(LocationIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
