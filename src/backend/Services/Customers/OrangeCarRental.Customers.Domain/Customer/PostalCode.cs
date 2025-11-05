using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Postal code value object.
///     Represents a German postal code (5 digits, e.g., "10115").
/// </summary>
/// <param name="Value">The postal code value.</param>
public readonly record struct PostalCode(string Value)
{
    public static PostalCode Of(string postalCode)
    {
        var trimmed = postalCode?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(postalCode))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(5, 5)
            .AndSatisfies(
                code => code.All(char.IsDigit),
                "German postal code must be exactly 5 digits");

        return new PostalCode(trimmed);
    }

    public static implicit operator string(PostalCode postalCode) => postalCode.Value;

    public override string ToString() => Value;
}
