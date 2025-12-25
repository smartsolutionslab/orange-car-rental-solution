using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

public interface ISepaMandateRepository : IRepository<SepaMandate, SepaMandateIdentifier>
{
    /// <summary>
    ///     Gets the active mandate for a customer.
    /// </summary>
    Task<SepaMandate?> GetActiveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all mandates for a customer.
    /// </summary>
    Task<IReadOnlyList<SepaMandate>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

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
}
