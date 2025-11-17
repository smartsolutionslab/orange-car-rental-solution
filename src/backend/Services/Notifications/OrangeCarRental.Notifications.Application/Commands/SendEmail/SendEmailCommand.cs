using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendEmail;

/// <summary>
///     Command to send an email notification.
/// </summary>
/// <param name="RecipientEmail">Recipient email address.</param>
/// <param name="Subject">Email subject.</param>
/// <param name="Body">Email body content (can be HTML).</param>
public sealed record SendEmailCommand(
    string RecipientEmail,
    string Subject,
    string Body) : ICommand<SendEmailResult>;
