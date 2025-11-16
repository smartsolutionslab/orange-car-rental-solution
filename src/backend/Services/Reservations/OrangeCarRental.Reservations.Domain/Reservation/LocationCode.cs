using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Represents a location code identifier (e.g., "BER-HBF", "MUC-APT").
///     References a location in the Fleet service where vehicles can be picked up or dropped off.
/// </summary>
/// <param name="Value">The location code value.</param>
public readonly record struct LocationCode(string Value)
{
    /// <summary>
    ///     Creates a new location code.
    /// </summary>
    /// <param name="code">The location code (3-20 characters, uppercase)</param>
    /// <returns>A new LocationCode instance</returns>
    /// <exception cref="ArgumentException">Thrown when code is invalid</exception>
    public static LocationCode Of(string code)
    {
        var trimmed = code.Trim().ToUpperInvariant();

        Ensure.That(trimmed, nameof(code))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(3, 20);

        return new LocationCode(trimmed);
    }

    /// <summary>
    ///     Implicit conversion from LocationCode to string for convenience.
    /// </summary>
    public static implicit operator string(LocationCode locationCode) => locationCode.Value;

    public override string ToString() => Value;
}
