using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Payments bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class PaymentsUnitOfWork(PaymentsDbContext context) : IPaymentsUnitOfWork
{
    /// <inheritdoc />
    public IPaymentRepository Payments =>
        field ??= new PaymentRepository(context);

    /// <inheritdoc />
    public IInvoiceRepository Invoices =>
        field ??= new InvoiceRepository(context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
