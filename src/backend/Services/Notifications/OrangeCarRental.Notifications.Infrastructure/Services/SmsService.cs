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

        // Log the SMS send operation (stub implementation)
        // Note: Phone numbers are not logged to prevent exposure of PII
        logger.LogInformation(
            "STUB: Sending SMS. Provider ID: {ProviderId}",
            providerMessageId);

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
}
