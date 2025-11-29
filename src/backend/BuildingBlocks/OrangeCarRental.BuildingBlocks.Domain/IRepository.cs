namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Repository interface for aggregate roots providing both read and write operations.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TIdentifier">The identifier type.</typeparam>
public interface IRepository<TAggregate, in TIdentifier>
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

    /// <summary>
    ///     Adds a new aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes an aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
}
