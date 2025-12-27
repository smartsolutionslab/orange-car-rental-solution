using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;

/// <summary>
///     Service for sending invoices via email.
/// </summary>
public interface IInvoiceEmailSender
{
    /// <summary>
    ///     Sends an invoice to the customer via email.
    /// </summary>
    /// <param name="invoice">The invoice to send.</param>
    /// <param name="recipientEmail">Customer's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the email sending operation.</returns>
    Task<SendInvoiceEmailResult> SendInvoiceAsync(
        Invoice invoice,
        string recipientEmail,
        CancellationToken cancellationToken = default);
}

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
