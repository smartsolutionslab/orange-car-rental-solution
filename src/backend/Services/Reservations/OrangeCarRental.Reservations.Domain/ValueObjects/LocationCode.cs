namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

/// <summary>
/// Represents a location code identifier (e.g., "BER-HBF", "MUC-APT").
/// References a location in the Fleet service where vehicles can be picked up or dropped off.
/// </summary>
public readonly record struct LocationCode
{
    public string Value { get; }

    private LocationCode(string value) => Value = value;

    /// <summary>
    /// Creates a new location code.
    /// </summary>
    /// <param name="code">The location code (3-20 characters, uppercase)</param>
    /// <returns>A new LocationCode instance</returns>
    /// <exception cref="ArgumentException">Thrown when code is invalid</exception>
    public static LocationCode Of(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        var trimmed = code.Trim().ToUpperInvariant();

        if (trimmed.Length < 3)
            throw new ArgumentException("Location code must be at least 3 characters long", nameof(code));

        if (trimmed.Length > 20)
            throw new ArgumentException("Location code cannot exceed 20 characters", nameof(code));

        return new LocationCode(trimmed);
    }

    /// <summary>
    /// Implicit conversion from LocationCode to string for convenience.
    /// </summary>
    public static implicit operator string(LocationCode locationCode) => locationCode.Value;

    public override string ToString() => Value;
}
