using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Services;

/// <summary>
///     Email service implementation.
///     Currently a stub/mock implementation that logs emails instead of sending them.
///     TODO: Replace with real email provider (e.g., SendGrid, AWS SES, Azure Communication Services).
/// </summary>
public sealed class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task<string> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        // Generate a mock provider message ID
        var providerMessageId = $"email-{Guid.NewGuid():N}";

        // Log the email (stub implementation)
        logger.LogInformation(
            "STUB: Sending email to {ToEmail} with subject '{Subject}'. Provider ID: {ProviderId}",
            toEmail,
            subject,
            providerMessageId);

        logger.LogDebug("Email body:\n{Body}", body);

        // Simulate async operation
        await Task.Delay(100, cancellationToken);

        // TODO: Implement real email sending
        // Example with SendGrid:
        // var client = new SendGridClient(_apiKey);
        // var from = new EmailAddress("noreply@orangecarrental.de", "Orange Car Rental");
        // var to = new EmailAddress(toEmail);
        // var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
        // var response = await client.SendEmailAsync(msg, cancellationToken);
        // return response.Headers.GetValues("X-Message-Id").FirstOrDefault();

        return providerMessageId;
    }
}
