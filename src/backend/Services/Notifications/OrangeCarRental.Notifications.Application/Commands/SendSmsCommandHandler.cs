using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Handler for SendSmsCommand.
///     Sends an SMS notification and tracks its status.
/// </summary>
public sealed class SendSmsCommandHandler(
    INotificationsUnitOfWork unitOfWork,
    ISmsService smsService)
    : ICommandHandler<SendSmsCommand, SendSmsResult>
{
    /// <summary>
    ///     Handles the send SMS command.
    /// </summary>
    /// <param name="command">The command with SMS details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Send SMS result with notification ID and status.</returns>
    public async Task<SendSmsResult> HandleAsync(
        SendSmsCommand command,
        CancellationToken cancellationToken = default)
    {
        // Create value objects
        var recipientPhone = RecipientPhone.From(command.RecipientPhone);
        var content = NotificationContent.From(command.Message);

        // Create notification aggregate
        var notification = Notification.CreateSms(recipientPhone, content);

        // Persist to repository
        var notifications = unitOfWork.Notifications;
        await notifications.AddAsync(notification, cancellationToken);

        try
        {
            // Send SMS via SMS service
            var providerMessageId = await smsService.SendSmsAsync(
                command.RecipientPhone,
                command.Message,
                cancellationToken);

            // Mark as sent
            notification = notification.MarkAsSent(providerMessageId);

            // Update in repository
            await notifications.UpdateAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new SendSmsResult(
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

            throw new InvalidOperationException($"Failed to send SMS: {ex.Message}", ex);
        }
    }
}
