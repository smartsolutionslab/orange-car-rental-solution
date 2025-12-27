using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Notification aggregate root.
///     Represents an email or SMS notification sent to a customer.
/// </summary>
public sealed class Notification : AggregateRoot<NotificationIdentifier>
{
    // For EF Core
    private Notification()
    {
        Subject = default;
        Content = default;
    }

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public NotificationType Type { get; init; }
    public RecipientEmail? RecipientEmail { get; init; }
    public RecipientPhone? RecipientPhone { get; init; }
    public NotificationSubject Subject { get; init; }
    public NotificationContent Content { get; init; }
    public NotificationStatus Status { get; init; }
    public string? ProviderMessageId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? SentAt { get; init; }
    public DateTime? DeliveredAt { get; init; }

    /// <summary>
    ///     Creates a new email notification.
    /// </summary>
    public static Notification CreateEmail(
        RecipientEmail recipientEmail,
        NotificationSubject subject,
        NotificationContent content)
    {
        return new Notification
        {
            Id = NotificationIdentifier.New(),
            Type = NotificationType.Email,
            RecipientEmail = recipientEmail,
            Subject = subject,
            Content = content,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     Creates a new SMS notification.
    /// </summary>
    public static Notification CreateSms(
        RecipientPhone recipientPhone,
        NotificationContent content)
    {
        return new Notification
        {
            Id = NotificationIdentifier.New(),
            Type = NotificationType.Sms,
            RecipientPhone = recipientPhone,
            Subject = NotificationSubject.From("SMS"), // SMS doesn't have subject
            Content = content,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    /// </summary>
    private Notification CreateMutatedCopy(
        NotificationStatus? status = null,
        string? providerMessageId = null,
        string? errorMessage = null,
        DateTime? sentAt = null,
        DateTime? deliveredAt = null)
    {
        return new Notification
        {
            Id = Id,
            Type = Type,
            RecipientEmail = RecipientEmail,
            RecipientPhone = RecipientPhone,
            Subject = Subject,
            Content = Content,
            Status = status ?? Status,
            ProviderMessageId = providerMessageId ?? ProviderMessageId,
            ErrorMessage = errorMessage ?? ErrorMessage,
            CreatedAt = CreatedAt,
            SentAt = sentAt ?? SentAt,
            DeliveredAt = deliveredAt ?? DeliveredAt
        };
    }

    /// <summary>
    ///     Marks the notification as sent.
    ///     Returns a new instance with sent status (immutable pattern).
    /// </summary>
    /// <param name="providerMessageId">External provider message ID.</param>
    public Notification MarkAsSent(string? providerMessageId = null)
    {
        return CreateMutatedCopy(
            status: NotificationStatus.Sent,
            providerMessageId: providerMessageId,
            sentAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the notification as delivered.
    ///     Returns a new instance with delivered status (immutable pattern).
    /// </summary>
    public Notification MarkAsDelivered()
    {
        return CreateMutatedCopy(
            status: NotificationStatus.Delivered,
            deliveredAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the notification as failed.
    ///     Returns a new instance with failed status (immutable pattern).
    /// </summary>
    /// <param name="errorMessage">Error message describing the failure.</param>
    public Notification MarkAsFailed(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage, nameof(errorMessage));

        return CreateMutatedCopy(
            status: NotificationStatus.Failed,
            errorMessage: errorMessage);
    }
}
