namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Strongly-typed identifier for Vehicle aggregate.
///     Uses GUID v7 for time-ordered identifiers with better database performance.
/// </summary>
public readonly record struct VehicleIdentifier(Guid Value)
{
    /// <summary>
    ///     Creates a new unique vehicle identifier using GUID v7.
    /// </summary>
    public static VehicleIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a vehicle identifier from an existing GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the GUID is empty.</exception>
    public static VehicleIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new VehicleIdentifier(value);
    }

    /// <summary>
    ///     Creates a vehicle identifier from a string representation of a GUID.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid GUID.</exception>
    public static VehicleIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"Invalid vehicle ID format: {value}", nameof(value));

        return From(guid);
    }

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(VehicleIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
