using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Unique identifier for Location aggregate.
///     Uses GUID v7 for time-ordered IDs.
/// </summary>
public readonly record struct LocationIdentifier(Guid Value) : IValueObject
{
    public static LocationIdentifier New() => new(Guid.CreateVersion7());

    public static LocationIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new LocationIdentifier(value);
    }

    public static implicit operator Guid(LocationIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
