using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

/// <summary>
///     EF Core implementation of IInvoiceRepository.
/// </summary>
public sealed class InvoiceRepository(PaymentsDbContext dbContext) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(InvoiceIdentifier id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(InvoiceNumber invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await dbContext.Invoices
            .FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Invoices
            .Where(x => x.Customer.CustomerId == customerId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice?> GetByReservationIdAsync(ReservationId reservationId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Invoices
            .FirstOrDefaultAsync(x => x.ReservationId == reservationId, cancellationToken);
    }

    public async Task<int> GetNextSequenceNumberAsync(int year, CancellationToken cancellationToken = default)
    {
        var maxSequence = await dbContext.Invoices
            .Where(x => x.InvoiceNumber.Value.Contains($"-{year}-"))
            .Select(x => x.InvoiceNumber)
            .ToListAsync(cancellationToken);

        if (maxSequence.Count == 0)
            return 1;

        // Parse sequence numbers and find max
        var maxNumber = maxSequence
            .Select(n => n.SequenceNumber)
            .Max();

        return maxNumber + 1;
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await dbContext.Invoices.AddAsync(invoice, cancellationToken);
    }

    public Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        dbContext.Invoices.Update(invoice);
        return Task.CompletedTask;
    }
}
