using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendSms;

/// <summary>
///     Command to send an SMS notification.
/// </summary>
/// <param name="RecipientPhone">Recipient phone number.</param>
/// <param name="Message">SMS message content.</param>
public sealed record SendSmsCommand(
    string RecipientPhone,
    string Message) : ICommand<SendSmsResult>;
