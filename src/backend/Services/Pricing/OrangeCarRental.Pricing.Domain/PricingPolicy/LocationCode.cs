namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
/// Location code for pricing (e.g., MUC-HBF, BER-AIRPORT).
/// Maps to Location from Fleet service.
/// </summary>
public readonly record struct LocationCode
{
    public string Value { get; }

    private LocationCode(string value) => Value = value;

    public static LocationCode Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        var trimmed = value.Trim().ToUpperInvariant();
        if (trimmed.Length < 3)
            throw new ArgumentException("Location code must be at least 3 characters long", nameof(value));

        if (trimmed.Length > 20)
            throw new ArgumentException("Location code cannot exceed 20 characters", nameof(value));

        return new LocationCode(trimmed);
    }

    public static implicit operator string(LocationCode code) => code.Value;

    public override string ToString() => Value;
}
