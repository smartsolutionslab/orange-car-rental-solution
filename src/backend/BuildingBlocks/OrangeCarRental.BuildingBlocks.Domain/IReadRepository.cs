namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Read-only repository interface for aggregate roots.
///     Use this interface when only read access is needed.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TIdentifier">The identifier type.</typeparam>
public interface IReadRepository<TAggregate, in TIdentifier>
    where TAggregate : AggregateRoot<TIdentifier>
    where TIdentifier : notnull
{
    /// <summary>
    ///     Gets an aggregate by its identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate.</returns>
    /// <exception cref="Exceptions.EntityNotFoundException">Thrown when the aggregate is not found.</exception>
    Task<TAggregate> GetByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
}
