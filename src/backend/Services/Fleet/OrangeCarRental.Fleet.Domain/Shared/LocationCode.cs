using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
/// Location code value object.
/// Represents a unique identifier for a rental location (e.g., "BER-HBF", "MUC-FLG").
/// </summary>
public readonly record struct LocationCode
{
    public string Value { get; }

    private LocationCode(string value)
    {
        Value = value;
    }

    public static LocationCode Of(string code)
    {
        var trimmed = code?.Trim().ToUpperInvariant() ?? string.Empty;

        Ensure.That(trimmed, nameof(code))
            .IsNotNullOrWhiteSpace()
            .AndHasMinLength(3);

        return new LocationCode(trimmed);
    }

    public static implicit operator string(LocationCode code) => code.Value;

    public override string ToString() => Value;
}
