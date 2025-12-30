using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Command to send an email notification.
/// </summary>
/// <param name="RecipientEmail">Recipient email address.</param>
/// <param name="Subject">Email subject.</param>
/// <param name="Body">Email body content (can be HTML).</param>
public sealed record SendEmailCommand(
    RecipientEmail RecipientEmail,
    NotificationSubject Subject,
    NotificationContent Body) : ICommand<SendEmailResult>;
