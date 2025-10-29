using System.Text.RegularExpressions;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Represents an email address with validation.
/// Email addresses are stored in lowercase for consistency.
/// </summary>
public readonly record struct EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException($"Invalid email address format: {value}", nameof(value));

        return new EmailAddress(value.Trim().ToLowerInvariant());
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
        return new EmailAddress($"anonymized-{Guid.CreateVersion7()}@gdpr-deleted.local");
    }

    public override string ToString() => Value;
}
