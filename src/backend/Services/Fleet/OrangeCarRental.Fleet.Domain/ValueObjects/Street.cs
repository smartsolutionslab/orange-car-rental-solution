namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Street address value object.
/// Represents the street portion of an address (e.g., "Europaplatz 1").
/// </summary>
public readonly record struct Street
{
    public string Value { get; }

    private Street(string value)
    {
        Value = value;
    }

    public static Street Of(string street)
    {
        // Street can be empty/optional
        var trimmed = street?.Trim() ?? string.Empty;

        if (trimmed.Length > 200)
        {
            throw new ArgumentException("Street address cannot exceed 200 characters", nameof(street));
        }

        return new Street(trimmed);
    }

    public static Street Empty => new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static implicit operator string(Street street) => street.Value;

    public override string ToString() => Value;
}
