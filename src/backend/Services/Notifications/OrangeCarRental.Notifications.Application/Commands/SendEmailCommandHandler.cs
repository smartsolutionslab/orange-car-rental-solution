using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Handler for SendEmailCommand.
///     Sends an email notification and tracks its status.
/// </summary>
public sealed class SendEmailCommandHandler(
    INotificationsUnitOfWork unitOfWork,
    IEmailService emailService)
    : ICommandHandler<SendEmailCommand, SendEmailResult>
{
    /// <summary>
    ///     Handles the send email command.
    /// </summary>
    /// <param name="command">The command with email details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Send email result with notification ID and status.</returns>
    public async Task<SendEmailResult> HandleAsync(
        SendEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        // Create notification aggregate using value objects from command
        var notification = Notification.CreateEmail(
            command.RecipientEmail,
            command.Subject,
            command.Body);

        // Persist to repository
        var notifications = unitOfWork.Notifications;
        await notifications.AddAsync(notification, cancellationToken);

        try
        {
            // Send email via email service
            var providerMessageId = await emailService.SendEmailAsync(
                command.RecipientEmail.Value,
                command.Subject.Value,
                command.Body.Value,
                cancellationToken);

            // Mark as sent
            notification = notification.MarkAsSent(providerMessageId);

            // Update in repository
            await notifications.UpdateAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new SendEmailResult(
                notification.Id.Value,
                notification.Status.ToString(),
                providerMessageId,
                notification.SentAt!.Value);
        }
        catch (Exception ex)
        {
            // Mark as failed
            notification = notification.MarkAsFailed(ex.Message);
            await notifications.UpdateAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}
