namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Shared;

/// <summary>
///     DTO for invoice data.
/// </summary>
public sealed record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly ServiceDate,
    DateOnly DueDate,
    string Status,
    Guid CustomerId,
    string CustomerName,
    Guid ReservationId,
    IReadOnlyList<InvoiceLineItemDto> LineItems,
    decimal TotalNet,
    decimal TotalVat,
    decimal TotalGross,
    string CurrencyCode,
    DateTime CreatedAt,
    DateTime? SentAt,
    DateTime? PaidAt);

/// <summary>
///     DTO for invoice line item.
/// </summary>
public sealed record InvoiceLineItemDto(
    int Position,
    string Description,
    int Quantity,
    string Unit,
    decimal UnitPriceNet,
    decimal VatRate,
    decimal TotalNet,
    decimal TotalGross);
