using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     Postal code value object for invoice addresses.
///     Supports German postal codes (5 digits) and other formats.
/// </summary>
public readonly record struct PostalCode(string Value) : IValueObject
{
    public static PostalCode From(string postalCode)
    {
        var trimmed = postalCode?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(postalCode))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(20);

        return new PostalCode(trimmed);
    }

    public static implicit operator string(PostalCode postalCode) => postalCode.Value;

    public override string ToString() => Value;
}
