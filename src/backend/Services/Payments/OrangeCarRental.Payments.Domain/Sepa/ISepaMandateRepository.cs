using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

public interface ISepaMandateRepository : IRepository<SepaMandate, SepaMandateIdentifier>
{
    /// <summary>
    ///     Gets the active mandate for a customer.
    /// </summary>
    Task<SepaMandate?> GetActiveByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all mandates for a customer.
    /// </summary>
    Task<IReadOnlyList<SepaMandate>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a mandate by its reference.
    /// </summary>
    Task<SepaMandate?> GetByMandateReferenceAsync(MandateReference reference, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the next sequence number for mandate references.
    /// </summary>
    Task<int> GetNextSequenceNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all expired mandates that need to be marked as expired.
    /// </summary>
    Task<IReadOnlyList<SepaMandate>> GetExpiredMandatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams expired mandates that need to be marked as expired.
    ///     Memory-efficient alternative to GetExpiredMandatesAsync for batch processing.
    /// </summary>
    IAsyncEnumerable<SepaMandate> StreamExpiredMandatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all mandates for a customer.
    ///     Memory-efficient alternative to GetByCustomerIdAsync.
    /// </summary>
    IAsyncEnumerable<SepaMandate> StreamByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
}
