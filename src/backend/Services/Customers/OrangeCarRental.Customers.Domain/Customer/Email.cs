using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Email value object with RFC 5322 format validation.
///     Email addresses are stored in lowercase for consistency and case-insensitive comparison.
/// </summary>
/// <param name="Value">The email address value.</param>
public readonly record struct Email(string Value) : IValueObject
{
    /// <summary>
    ///     Creates an email value object from a string.
    /// </summary>
    /// <param name="value">The email address.</param>
    /// <exception cref="ArgumentException">Thrown when the email is invalid.</exception>
    public static Email Of(string value)
    {
        var normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;

        Ensure.That(normalized, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(254)
            .AndIsValidEmail();

        return new Email(normalized);
    }

    public static Email? TryParse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return Of(value);
    }

    /// <summary>
    ///     Creates an anonymized email address for GDPR compliance.
    ///     Used when a customer requests data deletion.
    /// </summary>
    public static Email Anonymized() => new($"anonymized-{Guid.CreateVersion7()}@gdpr-deleted.local");

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}
