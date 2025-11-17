namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendEmail;

/// <summary>
///     Result of sending an email notification.
/// </summary>
public sealed record SendEmailResult
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
    ///     When the email was sent (UTC).
    /// </summary>
    public required DateTime SentAtUtc { get; init; }
}
