namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Type of notification being sent.
/// </summary>
public enum NotificationType
{
    /// <summary>
    ///     Email notification.
    /// </summary>
    Email = 1,

    /// <summary>
    ///     SMS notification.
    /// </summary>
    Sms = 2,

    /// <summary>
    ///     Both email and SMS.
    /// </summary>
    Both = 3
}
