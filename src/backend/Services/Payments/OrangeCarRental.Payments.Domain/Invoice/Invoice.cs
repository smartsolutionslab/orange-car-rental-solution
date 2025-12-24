using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Invoice aggregate root.
///     Compliant with German invoice requirements (§14 UStG).
/// </summary>
public sealed class Invoice : AggregateRoot<InvoiceIdentifier>
{
    private readonly List<InvoiceLineItem> _lineItems = [];

    private Invoice()
    {
    }

    #region Seller Information (Impressum)

    /// <summary>
    ///     Seller company name.
    /// </summary>
    public string SellerName { get; init; } = "Orange Car Rental GmbH";

    /// <summary>
    ///     Seller street address.
    /// </summary>
    public string SellerStreet { get; init; } = "Musterstraße 123";

    /// <summary>
    ///     Seller postal code.
    /// </summary>
    public string SellerPostalCode { get; init; } = "10115";

    /// <summary>
    ///     Seller city.
    /// </summary>
    public string SellerCity { get; init; } = "Berlin";

    /// <summary>
    ///     Seller country.
    /// </summary>
    public string SellerCountry { get; init; } = "Deutschland";

    /// <summary>
    ///     Seller email.
    /// </summary>
    public string SellerEmail { get; init; } = "rechnung@orangecarrental.de";

    /// <summary>
    ///     Seller phone.
    /// </summary>
    public string SellerPhone { get; init; } = "+49 30 12345678";

    /// <summary>
    ///     Seller trade register number (Handelsregisternummer).
    /// </summary>
    public string TradeRegisterNumber { get; init; } = "HRB 123456 B";

    /// <summary>
    ///     Seller VAT ID (Umsatzsteuer-Identifikationsnummer).
    /// </summary>
    public string VatId { get; init; } = "DE123456789";

    /// <summary>
    ///     Seller tax number (Steuernummer).
    /// </summary>
    public string TaxNumber { get; init; } = "27/123/45678";

    /// <summary>
    ///     Managing director name (Geschäftsführer).
    /// </summary>
    public string ManagingDirector { get; init; } = "Max Mustermann";

    #endregion

    #region Invoice Details

    /// <summary>
    ///     Unique consecutive invoice number.
    /// </summary>
    public InvoiceNumber InvoiceNumber { get; init; } = null!;

    /// <summary>
    ///     Invoice date (Rechnungsdatum).
    /// </summary>
    public DateOnly InvoiceDate { get; init; }

    /// <summary>
    ///     Service/delivery date (Leistungsdatum).
    /// </summary>
    public DateOnly ServiceDate { get; init; }

    /// <summary>
    ///     Due date for payment (Fälligkeitsdatum).
    /// </summary>
    public DateOnly DueDate { get; init; }

    /// <summary>
    ///     Invoice status.
    /// </summary>
    public InvoiceStatus Status { get; init; }

    #endregion

    #region Customer Information

    /// <summary>
    ///     Customer ID.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    ///     Customer full name.
    /// </summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    ///     Customer street address.
    /// </summary>
    public string CustomerStreet { get; init; } = string.Empty;

    /// <summary>
    ///     Customer postal code.
    /// </summary>
    public string CustomerPostalCode { get; init; } = string.Empty;

    /// <summary>
    ///     Customer city.
    /// </summary>
    public string CustomerCity { get; init; } = string.Empty;

    /// <summary>
    ///     Customer country.
    /// </summary>
    public string CustomerCountry { get; init; } = "Deutschland";

    /// <summary>
    ///     Customer VAT ID (for B2B).
    /// </summary>
    public string? CustomerVatId { get; init; }

    #endregion

    #region Reservation Reference

    /// <summary>
    ///     Related reservation ID.
    /// </summary>
    public Guid ReservationId { get; init; }

    /// <summary>
    ///     Related payment ID.
    /// </summary>
    public Guid? PaymentId { get; init; }

    #endregion

    #region Line Items and Totals

    /// <summary>
    ///     Invoice line items.
    /// </summary>
    public IReadOnlyList<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    /// <summary>
    ///     Total net amount (before VAT).
    /// </summary>
    public decimal TotalNet => _lineItems.Sum(x => x.TotalNet);

    /// <summary>
    ///     Total VAT amount.
    /// </summary>
    public decimal TotalVat => _lineItems.Sum(x => x.VatAmount);

    /// <summary>
    ///     Total gross amount (after VAT).
    /// </summary>
    public decimal TotalGross => TotalNet + TotalVat;

    /// <summary>
    ///     Currency code.
    /// </summary>
    public string CurrencyCode { get; init; } = "EUR";

    #endregion

    #region Metadata

    /// <summary>
    ///     When the invoice was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     When the invoice was sent to customer.
    /// </summary>
    public DateTime? SentAt { get; init; }

    /// <summary>
    ///     When the invoice was paid.
    /// </summary>
    public DateTime? PaidAt { get; init; }

    /// <summary>
    ///     When the invoice was voided.
    /// </summary>
    public DateTime? VoidedAt { get; init; }

    /// <summary>
    ///     PDF document stored as bytes.
    /// </summary>
    public byte[]? PdfDocument { get; private set; }

    #endregion

    /// <summary>
    ///     Creates a new invoice for a reservation.
    /// </summary>
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
        var invoice = new Invoice
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
            CreatedAt = DateTime.UtcNow
        };

        invoice._lineItems.AddRange(lineItems);
        return invoice;
    }

    /// <summary>
    ///     Attaches the generated PDF document.
    /// </summary>
    public Invoice WithPdfDocument(byte[] pdfBytes)
    {
        PdfDocument = pdfBytes;
        return this;
    }

    /// <summary>
    ///     Creates a copy with updated status.
    /// </summary>
    private Invoice CreateMutatedCopy(
        InvoiceStatus? status = null,
        DateTime? sentAt = null,
        DateTime? paidAt = null,
        DateTime? voidedAt = null)
    {
        var copy = new Invoice
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
            CurrencyCode = CurrencyCode,
            CreatedAt = CreatedAt,
            SentAt = sentAt ?? SentAt,
            PaidAt = paidAt ?? PaidAt,
            VoidedAt = voidedAt ?? VoidedAt,
            PdfDocument = PdfDocument
        };
        copy._lineItems.AddRange(_lineItems);
        return copy;
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
    ///     Voids the invoice.
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
