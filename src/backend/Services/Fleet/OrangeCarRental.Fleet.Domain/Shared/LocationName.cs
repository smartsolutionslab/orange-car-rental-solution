namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
/// Location name value object.
/// Represents the display name/title of a rental location (e.g., "Berlin Hauptbahnhof").
/// </summary>
public readonly record struct LocationName
{
    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static LocationName Of(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        var trimmed = name.Trim();
        if (trimmed.Length > 100)
        {
            throw new ArgumentException("Location name cannot exceed 100 characters", nameof(name));
        }

        return new LocationName(trimmed);
    }

    public static implicit operator string(LocationName name) => name.Value;

    public override string ToString() => Value;
}
