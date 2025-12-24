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

        // Sanitize and mask user input for logging
        var maskedEmail = MaskEmail(toEmail);
        var sanitizedSubject = SanitizeLogValue(subject);

        // Log the email (stub implementation)
        logger.LogInformation(
            "STUB: Sending email to {ToEmail} with subject '{Subject}'. Provider ID: {ProviderId}",
            maskedEmail,
            sanitizedSubject,
            providerMessageId);

        logger.LogDebug("Email body: {Body}", SanitizeLogValue(body));

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

    /// <summary>
    ///     Masks an email address to prevent exposure of sensitive information.
    ///     Example: "john.doe@example.com" becomes "jo***@example.com"
    /// </summary>
    private static string MaskEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return "[empty]";

        var sanitized = SanitizeLogValue(email);
        var atIndex = sanitized.IndexOf('@');
        if (atIndex <= 0)
            return "***@[invalid]";

        var localPart = sanitized[..atIndex];
        var domain = sanitized[(atIndex + 1)..];
        var visibleChars = Math.Min(2, localPart.Length);
        var masked = localPart[..visibleChars] + "***";

        return $"{masked}@{domain}";
    }

    /// <summary>
    ///     Sanitizes a value for logging to prevent log forging attacks (CWE-117).
    ///     Removes newlines and other control characters that could be used to inject fake log entries.
    /// </summary>
    private static string SanitizeLogValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value
            .Replace("\r\n", " ")
            .Replace("\r", " ")
            .Replace("\n", " ");
    }
}
