using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Invoice aggregate root.
///     Compliant with German invoice requirements (§14 UStG).
/// </summary>
public sealed class Invoice : AggregateRoot<InvoiceIdentifier>
{
    private Invoice()
    {
        InvoiceNumber = null!;
        Seller = null!;
        Customer = null!;
        LineItems = [];
        Currency = Currency.EUR;
    }

    /// <summary>
    ///     Seller/company information (Impressum).
    /// </summary>
    public SellerInfo Seller { get; init; }

    /// <summary>
    ///     Invoice number.
    /// </summary>
    public InvoiceNumber InvoiceNumber { get; init; }

    /// <summary>
    ///     Invoice date (Rechnungsdatum).
    /// </summary>
    public DateOnly InvoiceDate { get; init; }

    /// <summary>
    ///     Service date (Leistungsdatum).
    /// </summary>
    public DateOnly ServiceDate { get; init; }

    /// <summary>
    ///     Payment due date (Zahlungsziel).
    /// </summary>
    public DateOnly DueDate { get; init; }

    /// <summary>
    ///     Invoice status.
    /// </summary>
    public InvoiceStatus Status { get; init; }

    /// <summary>
    ///     Customer billing information.
    /// </summary>
    public CustomerInvoiceInfo Customer { get; init; }

    /// <summary>
    ///     Referenced reservation ID.
    /// </summary>
    public ReservationId ReservationId { get; init; }

    /// <summary>
    ///     Referenced payment ID (optional).
    /// </summary>
    public PaymentIdentifier? PaymentId { get; init; }

    /// <summary>
    ///     Line items.
    /// </summary>
    public IReadOnlyList<InvoiceLineItem> LineItems { get; init; }

    /// <summary>
    ///     Total net amount.
    /// </summary>
    public decimal TotalNet => LineItems.Sum(x => x.TotalNet);

    /// <summary>
    ///     Total VAT amount.
    /// </summary>
    public decimal TotalVat => LineItems.Sum(x => x.VatAmount);

    /// <summary>
    ///     Total gross amount.
    /// </summary>
    public decimal TotalGross => TotalNet + TotalVat;

    /// <summary>
    ///     Currency.
    /// </summary>
    public Currency Currency { get; init; }

    /// <summary>
    ///     Created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Sent timestamp.
    /// </summary>
    public DateTime? SentAt { get; init; }

    /// <summary>
    ///     Paid timestamp.
    /// </summary>
    public DateTime? PaidAt { get; init; }

    /// <summary>
    ///     Voided timestamp.
    /// </summary>
    public DateTime? VoidedAt { get; init; }

    /// <summary>
    ///     PDF document bytes.
    /// </summary>
    public byte[]? PdfDocument { get; init; }

    /// <summary>
    ///     Creates a new invoice.
    /// </summary>
    public static Invoice Create(
        InvoiceNumber invoiceNumber,
        ReservationId reservationId,
        CustomerInvoiceInfo customer,
        IEnumerable<InvoiceLineItem> lineItems,
        DateOnly serviceDate,
        int paymentTermDays = 14,
        PaymentIdentifier? paymentId = null,
        SellerInfo? seller = null)
    {
        return new Invoice
        {
            Id = InvoiceIdentifier.New(),
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ServiceDate = serviceDate,
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(paymentTermDays)),
            Status = InvoiceStatus.Created,
            Seller = seller ?? SellerInfo.OrangeCarRentalDefault,
            Customer = customer,
            ReservationId = reservationId,
            PaymentId = paymentId,
            LineItems = lineItems.ToList(),
            Currency = Currency.EUR,
            CreatedAt = DateTime.UtcNow
        };
    }

    private Invoice CreateMutatedCopy(
        InvoiceStatus? status = null,
        DateTime? sentAt = null,
        DateTime? paidAt = null,
        DateTime? voidedAt = null,
        byte[]? pdfDocument = null)
    {
        return new Invoice
        {
            Id = Id,
            InvoiceNumber = InvoiceNumber,
            InvoiceDate = InvoiceDate,
            ServiceDate = ServiceDate,
            DueDate = DueDate,
            Status = status ?? Status,
            Seller = Seller,
            Customer = Customer,
            ReservationId = ReservationId,
            PaymentId = PaymentId,
            LineItems = LineItems,
            Currency = Currency,
            CreatedAt = CreatedAt,
            SentAt = sentAt ?? SentAt,
            PaidAt = paidAt ?? PaidAt,
            VoidedAt = voidedAt ?? VoidedAt,
            PdfDocument = pdfDocument ?? PdfDocument
        };
    }

    /// <summary>
    ///     Attaches a PDF document to the invoice.
    /// </summary>
    public Invoice WithPdfDocument(byte[] pdfBytes)
    {
        return CreateMutatedCopy(pdfDocument: pdfBytes);
    }

    /// <summary>
    ///     Marks the invoice as sent to customer.
    /// </summary>
    public Invoice MarkAsSent()
    {
        if (Status != InvoiceStatus.Created)
            throw new InvalidOperationException($"Cannot mark invoice as sent in status: {Status}");

        return CreateMutatedCopy(
            status: InvoiceStatus.Sent,
            sentAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the invoice as paid.
    /// </summary>
    public Invoice MarkAsPaid()
    {
        if (Status == InvoiceStatus.Voided)
            throw new InvalidOperationException("Cannot mark voided invoice as paid");

        return CreateMutatedCopy(
            status: InvoiceStatus.Paid,
            paidAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Voids the invoice (Stornierung).
    /// </summary>
    public Invoice Void()
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot void a paid invoice");

        return CreateMutatedCopy(
            status: InvoiceStatus.Voided,
            voidedAt: DateTime.UtcNow);
    }
}
