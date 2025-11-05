using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     City name value object.
///     Represents a city name (e.g., "Berlin", "München").
/// </summary>
/// <param name="Value">The city name value.</param>
public readonly record struct City(string Value)
{
    public static City Of(string city)
    {
        var trimmed = city?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(city))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new City(trimmed);
    }

    public static implicit operator string(City city) => city.Value;

    public override string ToString() => Value;
}
