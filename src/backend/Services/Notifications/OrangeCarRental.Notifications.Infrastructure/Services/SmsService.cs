using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Services;

/// <summary>
///     SMS service implementation.
///     Currently a stub/mock implementation that logs SMS messages instead of sending them.
///     TODO: Replace with real SMS provider (e.g., Twilio, AWS SNS, Azure Communication Services).
/// </summary>
public sealed class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public async Task<string> SendSmsAsync(
        string toPhone,
        string message,
        CancellationToken cancellationToken = default)
    {
        // Generate a mock provider message ID
        var providerMessageId = $"sms-{Guid.NewGuid():N}";

        // Mask phone number to prevent exposure of sensitive information
        var maskedPhone = MaskPhoneNumber(toPhone);

        // Log the SMS (stub implementation)
        logger.LogInformation(
            "STUB: Sending SMS to {ToPhone}. Provider ID: {ProviderId}",
            maskedPhone,
            providerMessageId);

        logger.LogDebug("SMS message: {Message}", SanitizeLogValue(message));

        // Simulate async operation
        await Task.Delay(100, cancellationToken);

        // TODO: Implement real SMS sending
        // Example with Twilio:
        // var messageResource = await MessageResource.CreateAsync(
        //     body: message,
        //     from: new PhoneNumber(_fromNumber),
        //     to: new PhoneNumber(toPhone));
        // return messageResource.Sid;

        return providerMessageId;
    }

    /// <summary>
    ///     Masks a phone number to prevent exposure of sensitive information.
    ///     Example: "+49151234567" becomes "+49***4567"
    /// </summary>
    private static string MaskPhoneNumber(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return "[empty]";

        var sanitized = SanitizeLogValue(phone);
        if (sanitized.Length <= 4)
            return "***";

        var visibleEnd = sanitized[^4..];
        var prefix = sanitized.Length > 6 ? sanitized[..3] : "";
        return $"{prefix}***{visibleEnd}";
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
