using System.Globalization;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Handles sending invoice emails with PDF attachments.
///     Uses German language templates for the email content.
/// </summary>
public sealed class SendInvoiceEmailCommandHandler(IEmailService emailService)
{
    private static readonly CultureInfo GermanCulture = new("de-DE");

    public async Task<SendInvoiceEmailResult> HandleAsync(
        SendInvoiceEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = $"Ihre Rechnung {command.InvoiceNumber} von Orange Car Rental";
            var body = GenerateEmailBody(command);
            var attachments = new List<EmailAttachment>
            {
                new(
                    FileName: $"Rechnung_{command.InvoiceNumber}.pdf",
                    Content: command.PdfDocument,
                    ContentType: "application/pdf")
            };

            var providerMessageId = await emailService.SendEmailWithAttachmentsAsync(
                command.RecipientEmail.Value,
                subject,
                body,
                attachments,
                cancellationToken);

            return new SendInvoiceEmailResult(
                Success: true,
                ProviderMessageId: providerMessageId,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            return new SendInvoiceEmailResult(
                Success: false,
                ProviderMessageId: null,
                ErrorMessage: ex.Message);
        }
    }

    private static string GenerateEmailBody(SendInvoiceEmailCommand command)
    {
        var invoiceDateFormatted = command.InvoiceDate.ToString("dd.MM.yyyy", GermanCulture);
        var totalAmountFormatted = command.TotalAmount.ToGermanString();

        return $$"""
            <!DOCTYPE html>
            <html lang="de">
            <head>
                <meta charset="UTF-8">
                <style>
                    body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                    .header { background-color: #FF6600; color: white; padding: 20px; text-align: center; }
                    .content { padding: 20px; }
                    .footer { background-color: #f5f5f5; padding: 15px; font-size: 12px; text-align: center; }
                    .highlight { background-color: #fff3e0; padding: 15px; border-left: 4px solid #FF6600; margin: 20px 0; }
                </style>
            </head>
            <body>
                <div class="header">
                    <h1>Orange Car Rental</h1>
                </div>
                <div class="content">
                    <p>Sehr geehrte(r) {{command.RecipientName.Value}},</p>

                    <p>vielen Dank für Ihre Buchung bei Orange Car Rental.</p>

                    <p>Anbei erhalten Sie Ihre Rechnung als PDF-Dokument:</p>

                    <div class="highlight">
                        <strong>Rechnungsnummer:</strong> {{command.InvoiceNumber}}<br>
                        <strong>Rechnungsdatum:</strong> {{invoiceDateFormatted}}<br>
                        <strong>Gesamtbetrag:</strong> {{totalAmountFormatted}}
                    </div>

                    <p>Bitte beachten Sie die auf der Rechnung angegebenen Zahlungsbedingungen.</p>

                    <p>Bei Fragen zu Ihrer Rechnung stehen wir Ihnen gerne zur Verfügung.</p>

                    <p>Mit freundlichen Grüßen,<br>
                    Ihr Orange Car Rental Team</p>
                </div>
                <div class="footer">
                    <p>Orange Car Rental GmbH | Musterstraße 123 | 10115 Berlin</p>
                    <p>Tel: +49 30 123456-0 | E-Mail: info@orangecarrental.de</p>
                    <p>USt-IdNr.: DE123456789 | Handelsregister: HRB 12345</p>
                </div>
            </body>
            </html>
            """;
    }
}
