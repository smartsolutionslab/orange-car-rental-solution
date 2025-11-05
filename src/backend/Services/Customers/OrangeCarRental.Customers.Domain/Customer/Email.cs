using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
/// Email value object with RFC 5322 format validation.
/// Email addresses are stored in lowercase for consistency and case-insensitive comparison.
/// </summary>
public readonly record struct Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>
    /// Creates an email value object from a string.
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

    /// <summary>
    /// Creates an anonymized email address for GDPR compliance.
    /// Used when a customer requests data deletion.
    /// </summary>
    public static Email Anonymized()
    {
        return new Email($"anonymized-{Guid.CreateVersion7()}@gdpr-deleted.local");
    }

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}
