using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Notification subject/title value object.
/// </summary>
public readonly record struct NotificationSubject(string Value) : IValueObject
{
    /// <summary>
    ///     Creates a notification subject from a string.
    /// </summary>
    /// <param name="value">The subject text.</param>
    /// <exception cref="ArgumentException">Thrown when subject is invalid.</exception>
    public static NotificationSubject Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(200);

        return new NotificationSubject(value.Trim());
    }

    public static implicit operator string(NotificationSubject subject) => subject.Value;

    public override string ToString() => Value;
}
