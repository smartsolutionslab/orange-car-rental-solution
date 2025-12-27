namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

/// <summary>
///     Interface for query handlers in CQRS pattern.
///     Query handlers process read operations that do not modify system state.
/// </summary>
/// <typeparam name="TQuery">The query type to handle.</typeparam>
/// <typeparam name="TResult">The result type returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    ///     Handles the query asynchronously.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query result.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
