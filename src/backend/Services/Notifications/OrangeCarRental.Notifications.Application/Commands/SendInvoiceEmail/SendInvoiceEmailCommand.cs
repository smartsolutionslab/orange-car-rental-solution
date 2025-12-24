using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendInvoiceEmail;

/// <summary>
///     Command to send an invoice email with PDF attachment.
/// </summary>
/// <param name="RecipientEmail">Recipient email address.</param>
/// <param name="RecipientName">Recipient name for personalization.</param>
/// <param name="InvoiceNumber">Invoice number for reference.</param>
/// <param name="InvoiceDate">Invoice date for reference.</param>
/// <param name="TotalAmount">Total invoice amount formatted with currency.</param>
/// <param name="PdfDocument">PDF document bytes.</param>
public sealed record SendInvoiceEmailCommand(
    string RecipientEmail,
    string RecipientName,
    string InvoiceNumber,
    DateOnly InvoiceDate,
    string TotalAmount,
    byte[] PdfDocument) : ICommand<SendInvoiceEmailResult>;
