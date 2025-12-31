using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Command to send an SMS notification.
/// </summary>
/// <param name="RecipientPhone">Recipient phone number.</param>
/// <param name="Message">SMS message content.</param>
public sealed record SendSmsCommand(
    RecipientPhone RecipientPhone,
    NotificationContent Message) : ICommand<SendSmsResult>;
