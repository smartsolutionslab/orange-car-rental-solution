using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     Street address value object for invoice addresses.
/// </summary>
public readonly record struct Street(string Value) : IValueObject
{
    public static Street From(string street)
    {
        var trimmed = street?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(street))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(200);

        return new Street(trimmed);
    }

    public static implicit operator string(Street street) => street.Value;

    public override string ToString() => Value;
}
