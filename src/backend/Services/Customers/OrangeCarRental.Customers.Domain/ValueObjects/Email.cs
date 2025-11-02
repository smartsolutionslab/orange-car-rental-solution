using System.Text.RegularExpressions;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

/// <summary>
/// Email value object with RFC 5322 format validation.
/// Email addresses are stored in lowercase for consistency and case-insensitive comparison.
/// </summary>
public readonly record struct Email
{
    // RFC 5322 compliant email regex pattern
    private static readonly Regex EmailRegex = new(
        @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>
    /// Creates an email value object from a string.
    /// </summary>
    /// <param name="value">The email address.</param>
    /// <exception cref="ArgumentException">Thrown when the email is invalid.</exception>
    public static Email Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!IsValidEmail(normalized))
            throw new ArgumentException($"Invalid email address format: {value}", nameof(value));

        // Additional length validation
        if (normalized.Length > 254) // RFC 5321
            throw new ArgumentException("Email address is too long (max 254 characters)", nameof(value));

        return new Email(normalized);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Check for basic structure first (faster)
        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
            return false;

        // Check that there's only one @ symbol
        if (email.LastIndexOf('@') != atIndex)
            return false;

        // Check for domain
        var domain = email.Substring(atIndex + 1);
        if (!domain.Contains('.'))
            return false;

        // Full RFC 5322 validation
        return EmailRegex.IsMatch(email);
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
