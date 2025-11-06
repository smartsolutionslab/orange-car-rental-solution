using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     First name value object with validation.
///     Names are trimmed and must be between 1 and 100 characters.
/// </summary>
/// <param name="Value">The first name value.</param>
public readonly record struct FirstName(string Value)
{
    /// <summary>
    ///     Creates a first name value object from a string.
    /// </summary>
    /// <param name="value">The first name.</param>
    /// <exception cref="ArgumentException">Thrown when the first name is invalid.</exception>
    public static FirstName Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        Ensure.That(normalized, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(1, 100)
            .AndSatisfies(
                name => !string.IsNullOrWhiteSpace(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-' || c == '\''),
                $"First name must contain only letters, spaces, hyphens, or apostrophes: {value}");

        return new FirstName(normalized);
    }

    /// <summary>
    ///     Creates an anonymized first name for GDPR compliance.
    ///     Used when a customer requests data deletion.
    /// </summary>
    public static FirstName Anonymized() => new("Anonymized");

    public static implicit operator string(FirstName firstName) => firstName.Value;

    public override string ToString() => Value;
}
