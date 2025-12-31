namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Result of sending an email notification.
/// </summary>
public sealed record SendEmailResult(
    Guid NotificationId,
    string Status,
    string? ProviderMessageId,
    DateTime SentAtUtc);
