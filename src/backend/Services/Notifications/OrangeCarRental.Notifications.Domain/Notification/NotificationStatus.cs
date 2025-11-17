namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Status of a notification.
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    ///     Notification is pending to be sent.
    /// </summary>
    Pending = 1,

    /// <summary>
    ///     Notification has been sent successfully.
    /// </summary>
    Sent = 2,

    /// <summary>
    ///     Notification delivery failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    ///     Notification was delivered to recipient.
    /// </summary>
    Delivered = 4
}
