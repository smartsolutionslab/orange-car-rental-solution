namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendSms;

/// <summary>
///     Result of sending an SMS notification.
/// </summary>
public sealed record SendSmsResult
{
    /// <summary>
    ///     Notification identifier.
    /// </summary>
    public required Guid NotificationId { get; init; }

    /// <summary>
    ///     Status of the notification.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    ///     Provider message ID (for tracking).
    /// </summary>
    public string? ProviderMessageId { get; init; }

    /// <summary>
    ///     When the SMS was sent (UTC).
    /// </summary>
    public required DateTime SentAtUtc { get; init; }
}
