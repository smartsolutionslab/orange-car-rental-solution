using System.Text.RegularExpressions;

namespace OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Represents an email address with validation.
/// Email addresses are stored in lowercase for consistency.
/// </summary>
public sealed record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; init; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException($"Invalid email address format: {value}", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        return EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// Creates an anonymized email address for GDPR compliance.
    /// </summary>
    public static EmailAddress Anonymized()
    {
        return new EmailAddress($"anonymized-{Guid.NewGuid()}@gdpr-deleted.local");
    }

    public override string ToString() => Value;
}
