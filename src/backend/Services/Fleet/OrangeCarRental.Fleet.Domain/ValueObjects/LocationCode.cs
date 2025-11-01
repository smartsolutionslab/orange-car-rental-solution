namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

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
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Location code cannot be empty", nameof(code));
        }

        // Validate format: XXX-XXX (3 letters, hyphen, 3 letters)
        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length < 3)
        {
            throw new ArgumentException("Location code must be at least 3 characters long", nameof(code));
        }

        return new LocationCode(trimmed);
    }

    public static implicit operator string(LocationCode code) => code.Value;

    public override string ToString() => Value;
}
