using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Strongly-typed identifier for Notification aggregate.
///     Uses GUID v7 for time-ordered identifiers.
/// </summary>
public readonly record struct NotificationIdentifier(Guid Value) : IValueObject
{
    /// <summary>
    ///     Creates a new notification identifier with a time-ordered GUID v7.
    /// </summary>
    public static NotificationIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a notification identifier from an existing GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentException">Thrown when value is empty GUID.</exception>
    public static NotificationIdentifier From(Guid value)
    {
        Ensure.That(value, nameof(value)).IsNotEmpty();
        return new NotificationIdentifier(value);
    }

    /// <summary>
    ///     Creates a notification identifier from a string.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <exception cref="ArgumentException">Thrown when value is not a valid GUID.</exception>
    public static NotificationIdentifier From(string value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(!Guid.TryParse(value, out var guid), $"Invalid notification ID format: {value}");

        return From(Guid.Parse(value));
    }

    public static implicit operator Guid(NotificationIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
