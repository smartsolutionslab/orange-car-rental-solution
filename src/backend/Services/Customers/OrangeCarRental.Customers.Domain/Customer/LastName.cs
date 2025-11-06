using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Last name value object with validation.
///     Names are trimmed and must be between 1 and 100 characters.
/// </summary>
/// <param name="Value">The last name value.</param>
public readonly record struct LastName(string Value)
{
    /// <summary>
    ///     Creates a last name value object from a string.
    /// </summary>
    /// <param name="value">The last name.</param>
    /// <exception cref="ArgumentException">Thrown when the last name is invalid.</exception>
    public static LastName Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        Ensure.That(normalized, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(1, 100)
            .AndSatisfies(
                name => !string.IsNullOrWhiteSpace(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-' || c == '\''),
                $"Last name must contain only letters, spaces, hyphens, or apostrophes: {value}");

        return new LastName(normalized);
    }

    /// <summary>
    ///     Creates an anonymized last name for GDPR compliance.
    ///     Used when a customer requests data deletion.
    /// </summary>
    public static LastName Anonymized() => new("Anonymized");

    public static implicit operator string(LastName lastName) => lastName.Value;

    public override string ToString() => Value;
}
