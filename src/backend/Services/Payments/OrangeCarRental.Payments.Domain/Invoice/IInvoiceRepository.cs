using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Repository interface for Invoice aggregate.
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    ///     Gets an invoice by its identifier.
    /// </summary>
    Task<Invoice?> GetByIdAsync(InvoiceIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets an invoice by its invoice number.
    /// </summary>
    Task<Invoice?> GetByInvoiceNumberAsync(InvoiceNumber invoiceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all invoices for a customer.
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all invoices for a customer.
    ///     Memory-efficient alternative to GetByCustomerIdAsync.
    /// </summary>
    IAsyncEnumerable<Invoice> StreamByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the invoice for a reservation.
    /// </summary>
    Task<Invoice?> GetByReservationIdAsync(ReservationId reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the next sequence number for invoice numbering in the given year.
    /// </summary>
    Task<int> GetNextSequenceNumberAsync(int year, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new invoice.
    /// </summary>
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing invoice.
    /// </summary>
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
