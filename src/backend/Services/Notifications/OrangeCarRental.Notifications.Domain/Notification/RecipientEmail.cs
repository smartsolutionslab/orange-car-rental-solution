using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Recipient email address value object.
/// </summary>
public readonly record struct RecipientEmail(string Value) : IValueObject
{
    /// <summary>
    ///     Creates a recipient email from a string.
    /// </summary>
    /// <param name="value">The email address.</param>
    /// <exception cref="ArgumentException">Thrown when email is invalid.</exception>
    public static RecipientEmail Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(255)
            .AndSatisfies(
                v => v.Contains('@') && v.Contains('.'),
                "Email must be in valid format (e.g. user@example.com)");

        return new RecipientEmail(value.ToLowerInvariant().Trim());
    }

    public static implicit operator string(RecipientEmail email) => email.Value;

    public override string ToString() => Value;
}
