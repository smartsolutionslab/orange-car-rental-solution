namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

/// <summary>
///     Result of invoice generation.
/// </summary>
public sealed record GenerateInvoiceResult(
    bool Success,
    Guid? InvoiceId,
    string? InvoiceNumber,
    byte[]? PdfDocument,
    string? ErrorMessage);
