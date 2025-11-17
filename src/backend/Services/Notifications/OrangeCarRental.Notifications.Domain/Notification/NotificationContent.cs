using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Notification content/body value object.
/// </summary>
public readonly record struct NotificationContent(string Value) : IValueObject
{
    /// <summary>
    ///     Creates notification content from a string.
    /// </summary>
    /// <param name="value">The content text.</param>
    /// <exception cref="ArgumentException">Thrown when content is invalid.</exception>
    public static NotificationContent Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(10000);

        return new NotificationContent(value.Trim());
    }

    public static implicit operator string(NotificationContent content) => content.Value;

    public override string ToString() => Value;
}
