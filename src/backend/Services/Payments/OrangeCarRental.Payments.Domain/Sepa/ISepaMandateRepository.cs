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
    ///     Streams all expired mandates asynchronously for batch processing.
    ///     Preferred over GetExpiredMandatesAsync for processing large numbers of expired mandates.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable stream of expired mandates.</returns>
    IAsyncEnumerable<SepaMandate> StreamExpiredMandatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all mandates for a customer asynchronously.
    ///     Preferred over GetByCustomerIdAsync for customers with large mandate histories.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable stream of mandates.</returns>
    IAsyncEnumerable<SepaMandate> StreamByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
}
