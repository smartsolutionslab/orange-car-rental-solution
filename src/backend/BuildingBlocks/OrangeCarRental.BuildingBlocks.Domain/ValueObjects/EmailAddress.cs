using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Represents an email address with validation.
/// Email addresses are stored in lowercase for consistency.
/// </summary>
public readonly record struct EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(256)
            .AndIsValidEmail();

        return new EmailAddress(value.Trim().ToLowerInvariant());
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
