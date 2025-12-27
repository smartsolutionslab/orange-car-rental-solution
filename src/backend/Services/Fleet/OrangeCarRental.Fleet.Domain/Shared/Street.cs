using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
///     Street address value object.
///     Represents the street portion of an address (e.g., "Europaplatz 1").
/// </summary>
public readonly record struct Street(string Value) : IValueObject
{
    public static Street Empty => new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static Street From(string street)
    {
        // Street can be empty/optional
        var trimmed = street?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(street))
            .ThrowIf(trimmed.Length > 200, "Street address cannot exceed 200 characters");

        return new Street(trimmed);
    }

    public static implicit operator string(Street street) => street.Value;

    public override string ToString() => Value;
}
