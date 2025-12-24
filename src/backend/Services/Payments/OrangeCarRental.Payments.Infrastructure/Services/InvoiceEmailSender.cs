using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Services;

/// <summary>
///     Service for sending invoices via email.
///     Currently a stub implementation that logs instead of sending.
///     TODO: Replace with real implementation (e.g., HTTP call to Notifications API or message queue).
/// </summary>
public sealed class InvoiceEmailSender(ILogger<InvoiceEmailSender> logger) : IInvoiceEmailSender
{
    private static readonly CultureInfo GermanCulture = new("de-DE");

    public async Task<SendInvoiceEmailResult> SendInvoiceAsync(
        Invoice invoice,
        string recipientEmail,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (invoice.PdfDocument == null || invoice.PdfDocument.Length == 0)
            {
                return new SendInvoiceEmailResult(
                    Success: false,
                    ProviderMessageId: null,
                    ErrorMessage: "Invoice PDF document is not available.");
            }

            // Generate email content
            var subject = $"Ihre Rechnung {invoice.InvoiceNumber} von Orange Car Rental";
            var body = GenerateEmailBody(invoice);
            var attachmentFileName = $"Rechnung_{invoice.InvoiceNumber.Value}.pdf";

            // Stub implementation - log instead of sending
            // Note: Email addresses are not logged to prevent exposure of PII
            logger.LogInformation(
                "STUB: Sending invoice email for {InvoiceNumber} with subject '{Subject}' and attachment '{FileName}' ({FileSize} bytes)",
                invoice.InvoiceNumber.Value,
                subject,
                attachmentFileName,
                invoice.PdfDocument.Length);

            // Simulate async operation
            await Task.Delay(100, cancellationToken);

            // TODO: Implement real email sending
            // Option 1: HTTP call to Notifications API
            // var httpClient = _httpClientFactory.CreateClient("Notifications");
            // var response = await httpClient.PostAsJsonAsync("/api/notifications/send-invoice-email", new
            // {
            //     RecipientEmail = recipientEmail,
            //     RecipientName = invoice.CustomerName,
            //     InvoiceNumber = invoice.InvoiceNumber.Value,
            //     InvoiceDate = invoice.InvoiceDate,
            //     TotalAmount = FormatCurrency(invoice.TotalGross, invoice.CurrencyCode),
            //     PdfDocument = Convert.ToBase64String(invoice.PdfDocument)
            // }, cancellationToken);

            // Option 2: Publish event to message queue (e.g., MassTransit, Azure Service Bus)
            // await _publishEndpoint.Publish(new InvoiceCreatedEvent
            // {
            //     InvoiceId = invoice.Id.Value,
            //     RecipientEmail = recipientEmail,
            //     ...
            // }, cancellationToken);

            var providerMessageId = $"invoice-email-{Guid.NewGuid():N}";

            return new SendInvoiceEmailResult(
                Success: true,
                ProviderMessageId: providerMessageId,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send invoice email for {InvoiceNumber}", invoice.InvoiceNumber.Value);

            return new SendInvoiceEmailResult(
                Success: false,
                ProviderMessageId: null,
                ErrorMessage: ex.Message);
        }
    }

    private static string GenerateEmailBody(Invoice invoice)
    {
        var invoiceDateFormatted = invoice.InvoiceDate.ToString("dd.MM.yyyy", GermanCulture);
        var dueDateFormatted = invoice.DueDate.ToString("dd.MM.yyyy", GermanCulture);
        var totalFormatted = FormatCurrency(invoice.TotalGross, invoice.CurrencyCode);

        return $$"""
            <!DOCTYPE html>
            <html lang="de">
            <head>
                <meta charset="UTF-8">
                <style>
                    body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; }
                    .header { background-color: #FF6600; color: white; padding: 20px; text-align: center; }
                    .header h1 { margin: 0; }
                    .content { padding: 20px; }
                    .footer { background-color: #f5f5f5; padding: 15px; font-size: 12px; text-align: center; color: #666; }
                    .highlight { background-color: #fff3e0; padding: 15px; border-left: 4px solid #FF6600; margin: 20px 0; }
                    .highlight strong { display: inline-block; width: 150px; }
                </style>
            </head>
            <body>
                <div class="header">
                    <h1>Orange Car Rental</h1>
                    <p style="margin: 5px 0 0 0;">Autovermietung</p>
                </div>
                <div class="content">
                    <p>Sehr geehrte(r) {{invoice.CustomerName}},</p>

                    <p>vielen Dank für Ihre Buchung bei Orange Car Rental.</p>

                    <p>Anbei erhalten Sie Ihre Rechnung als PDF-Dokument:</p>

                    <div class="highlight">
                        <strong>Rechnungsnummer:</strong> {{invoice.InvoiceNumber.Value}}<br>
                        <strong>Rechnungsdatum:</strong> {{invoiceDateFormatted}}<br>
                        <strong>Fällig am:</strong> {{dueDateFormatted}}<br>
                        <strong>Gesamtbetrag:</strong> {{totalFormatted}}
                    </div>

                    <p>Bitte beachten Sie die auf der Rechnung angegebenen Zahlungsbedingungen und überweisen Sie den
                    Betrag bis zum {{dueDateFormatted}} auf das angegebene Konto.</p>

                    <p>Bei Fragen zu Ihrer Rechnung stehen wir Ihnen gerne zur Verfügung:</p>
                    <ul>
                        <li>Telefon: +49 30 123456-0</li>
                        <li>E-Mail: buchhaltung@orangecarrental.de</li>
                    </ul>

                    <p>Mit freundlichen Grüßen,<br>
                    <strong>Ihr Orange Car Rental Team</strong></p>
                </div>
                <div class="footer">
                    <p><strong>{{invoice.SellerName}}</strong></p>
                    <p>{{invoice.SellerStreet}} | {{invoice.SellerPostalCode}} {{invoice.SellerCity}}</p>
                    <p>Tel: {{invoice.SellerPhone}} | E-Mail: {{invoice.SellerEmail}}</p>
                    <p>USt-IdNr.: {{invoice.VatId}} | Handelsregister: AG Berlin {{invoice.TradeRegisterNumber}}</p>
                    <p style="margin-top: 10px; font-size: 10px;">
                        Diese E-Mail wurde automatisch erstellt. Bitte antworten Sie nicht direkt auf diese Nachricht.
                    </p>
                </div>
            </body>
            </html>
            """;
    }

    private static string FormatCurrency(decimal amount, string currencyCode)
    {
        return currencyCode.ToUpperInvariant() switch
        {
            "EUR" => $"{amount.ToString("N2", GermanCulture)} €",
            "USD" => $"${amount.ToString("N2", GermanCulture)}",
            _ => $"{amount.ToString("N2", GermanCulture)} {currencyCode}"
        };
    }
}
