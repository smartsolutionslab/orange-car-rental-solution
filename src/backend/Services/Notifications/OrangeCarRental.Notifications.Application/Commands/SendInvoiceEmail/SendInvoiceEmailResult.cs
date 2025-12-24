namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendInvoiceEmail;

/// <summary>
///     Result of sending an invoice email.
/// </summary>
/// <param name="Success">Whether the email was sent successfully.</param>
/// <param name="ProviderMessageId">Message ID from the email provider.</param>
/// <param name="ErrorMessage">Error message if sending failed.</param>
public sealed record SendInvoiceEmailResult(
    bool Success,
    string? ProviderMessageId,
    string? ErrorMessage);
