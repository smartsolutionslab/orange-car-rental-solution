namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Write repository interface for aggregate roots.
///     Use this interface when write operations are needed.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TIdentifier">The identifier type.</typeparam>
public interface IWriteRepository<TAggregate, in TIdentifier>
    where TAggregate : AggregateRoot<TIdentifier>
    where TIdentifier : notnull
{
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
