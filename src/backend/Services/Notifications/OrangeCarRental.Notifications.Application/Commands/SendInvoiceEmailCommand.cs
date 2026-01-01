using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

/// <summary>
///     Command to send an invoice email with PDF attachment.
/// </summary>
/// <param name="RecipientEmail">Recipient email address.</param>
/// <param name="RecipientName">Recipient name for personalization.</param>
/// <param name="InvoiceNumber">Invoice number for reference.</param>
/// <param name="InvoiceDate">Invoice date for reference.</param>
/// <param name="TotalAmount">Total invoice amount.</param>
/// <param name="PdfDocument">PDF document bytes.</param>
public sealed record SendInvoiceEmailCommand(
    RecipientEmail RecipientEmail,
    PersonName RecipientName,
    string InvoiceNumber,
    DateOnly InvoiceDate,
    Money TotalAmount,
    byte[] PdfDocument) : ICommand<SendInvoiceEmailResult>;
