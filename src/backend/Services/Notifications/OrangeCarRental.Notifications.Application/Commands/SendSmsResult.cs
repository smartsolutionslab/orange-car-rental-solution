namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Result of sending an SMS notification.
/// </summary>
public sealed record SendSmsResult(
    Guid NotificationId,
    string Status,
    string? ProviderMessageId,
    DateTime SentAtUtc);
