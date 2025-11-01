namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// City name value object.
/// Represents a city name (e.g., "Berlin", "München").
/// </summary>
public readonly record struct City
{
    public string Value { get; }

    private City(string value)
    {
        Value = value;
    }

    public static City Of(string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));

        var trimmed = city.Trim();
        if (trimmed.Length > 100) throw new ArgumentException("City name cannot exceed 100 characters", nameof(city));

        return new City(trimmed);
    }

    public static implicit operator string(City city) => city.Value;

    public override string ToString() => Value;
}
