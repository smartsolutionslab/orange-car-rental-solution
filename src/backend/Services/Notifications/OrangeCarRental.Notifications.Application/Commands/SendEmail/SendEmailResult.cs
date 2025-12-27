namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendEmail;

/// <summary>
///     Result of sending an email notification.
/// </summary>
public sealed record SendEmailResult(
    Guid NotificationId,
    string Status,
    string? ProviderMessageId,
    DateTime SentAtUtc);
