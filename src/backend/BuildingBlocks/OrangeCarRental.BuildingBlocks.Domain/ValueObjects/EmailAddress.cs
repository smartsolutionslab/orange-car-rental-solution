using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Represents an email address with validation.
///     Email addresses are stored in lowercase for consistency.
/// </summary>
/// <param name="Value">The email address value.</param>
public readonly record struct EmailAddress(string Value) : IValueObject
{
    public static EmailAddress Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(256)
            .AndIsValidEmail();

        return new EmailAddress(value.Trim().ToLowerInvariant());
    }

    /// <summary>
    ///     Creates an anonymized email address for GDPR compliance.
    /// </summary>
    public static EmailAddress Anonymized() => new($"anonymized-{Guid.CreateVersion7()}@gdpr-deleted.local");

    public override string ToString() => Value;
}
