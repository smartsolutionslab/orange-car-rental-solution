using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Recipient phone number value object.
///     Supports German phone format (+49XXXXXXXXXX).
/// </summary>
public readonly record struct RecipientPhone(string Value) : IValueObject
{
    /// <summary>
    ///     Creates a recipient phone from a string.
    /// </summary>
    /// <param name="value">The phone number.</param>
    /// <exception cref="ArgumentException">Thrown when phone is invalid.</exception>
    public static RecipientPhone From(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace();

        // Remove common formatting characters
        var normalized = value.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("/", "");

        // Convert German domestic format to international
        if (normalized.StartsWith("0") && !normalized.StartsWith("00"))
            normalized = "+49" + normalized.Substring(1);
        else if (normalized.StartsWith("0049"))
            normalized = "+49" + normalized.Substring(4);

        Ensure.That(normalized, nameof(value))
            .AndStartsWith("+49")
            .AndHasLengthBetween(6, 16);

        return new RecipientPhone(normalized);
    }

    public static implicit operator string(RecipientPhone phone) => phone.Value;

    public override string ToString() => Value;
}
