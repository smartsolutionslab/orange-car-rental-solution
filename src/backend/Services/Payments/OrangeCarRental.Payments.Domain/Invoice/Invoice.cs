using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

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
        LineItems = [];
    }

    // Seller Information (Impressum)
    public string SellerName { get; init; } = "Orange Car Rental GmbH";
    public string SellerStreet { get; init; } = "Musterstraße 123";
    public string SellerPostalCode { get; init; } = "10115";
    public string SellerCity { get; init; } = "Berlin";
    public string SellerCountry { get; init; } = "Deutschland";
    public string SellerEmail { get; init; } = "rechnung@orangecarrental.de";
    public string SellerPhone { get; init; } = "+49 30 12345678";
    public string TradeRegisterNumber { get; init; } = "HRB 123456 B";
    public string VatId { get; init; } = "DE123456789";
    public string TaxNumber { get; init; } = "27/123/45678";
    public string ManagingDirector { get; init; } = "Max Mustermann";

    // Invoice Details
    public InvoiceNumber InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly ServiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public InvoiceStatus Status { get; init; }

    // Customer Information
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerStreet { get; init; } = string.Empty;
    public string CustomerPostalCode { get; init; } = string.Empty;
    public string CustomerCity { get; init; } = string.Empty;
    public string CustomerCountry { get; init; } = "Deutschland";
    public string? CustomerVatId { get; init; }

    // Reservation Reference
    public Guid ReservationId { get; init; }
    public Guid? PaymentId { get; init; }

    // Line Items and Totals
    public IReadOnlyList<InvoiceLineItem> LineItems { get; init; }
    public decimal TotalNet => LineItems.Sum(x => x.TotalNet);
    public decimal TotalVat => LineItems.Sum(x => x.VatAmount);
    public decimal TotalGross => TotalNet + TotalVat;
    public string CurrencyCode { get; init; } = "EUR";

    // Metadata
    public DateTime CreatedAt { get; init; }
    public DateTime? SentAt { get; init; }
    public DateTime? PaidAt { get; init; }
    public DateTime? VoidedAt { get; init; }
    public byte[]? PdfDocument { get; init; }

    public static Invoice Create(
        InvoiceNumber invoiceNumber,
        Guid reservationId,
        Guid customerId,
        string customerName,
        string customerStreet,
        string customerPostalCode,
        string customerCity,
        string customerCountry,
        string? customerVatId,
        IEnumerable<InvoiceLineItem> lineItems,
        DateOnly serviceDate,
        int paymentTermDays = 14,
        Guid? paymentId = null)
    {
        return new Invoice
        {
            Id = InvoiceIdentifier.New(),
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ServiceDate = serviceDate,
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(paymentTermDays)),
            Status = InvoiceStatus.Created,
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerStreet = customerStreet,
            CustomerPostalCode = customerPostalCode,
            CustomerCity = customerCity,
            CustomerCountry = customerCountry,
            CustomerVatId = customerVatId,
            ReservationId = reservationId,
            PaymentId = paymentId,
            LineItems = lineItems.ToList(),
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
            SellerName = SellerName,
            SellerStreet = SellerStreet,
            SellerPostalCode = SellerPostalCode,
            SellerCity = SellerCity,
            SellerCountry = SellerCountry,
            SellerEmail = SellerEmail,
            SellerPhone = SellerPhone,
            TradeRegisterNumber = TradeRegisterNumber,
            VatId = VatId,
            TaxNumber = TaxNumber,
            ManagingDirector = ManagingDirector,
            CustomerId = CustomerId,
            CustomerName = CustomerName,
            CustomerStreet = CustomerStreet,
            CustomerPostalCode = CustomerPostalCode,
            CustomerCity = CustomerCity,
            CustomerCountry = CustomerCountry,
            CustomerVatId = CustomerVatId,
            ReservationId = ReservationId,
            PaymentId = PaymentId,
            LineItems = LineItems,
            CurrencyCode = CurrencyCode,
            CreatedAt = CreatedAt,
            SentAt = sentAt ?? SentAt,
            PaidAt = paidAt ?? PaidAt,
            VoidedAt = voidedAt ?? VoidedAt,
            PdfDocument = pdfDocument ?? PdfDocument
        };
    }

    public Invoice WithPdfDocument(byte[] pdfBytes)
    {
        return CreateMutatedCopy(pdfDocument: pdfBytes);
    }

    public Invoice MarkAsSent()
    {
        if (Status != InvoiceStatus.Created)
            throw new InvalidOperationException($"Cannot mark invoice as sent in status: {Status}");

        return CreateMutatedCopy(
            status: InvoiceStatus.Sent,
            sentAt: DateTime.UtcNow);
    }

    public Invoice MarkAsPaid()
    {
        if (Status == InvoiceStatus.Voided)
            throw new InvalidOperationException("Cannot mark voided invoice as paid");

        return CreateMutatedCopy(
            status: InvoiceStatus.Paid,
            paidAt: DateTime.UtcNow);
    }

    public Invoice Void()
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot void a paid invoice");

        return CreateMutatedCopy(
            status: InvoiceStatus.Voided,
            voidedAt: DateTime.UtcNow);
    }
}
