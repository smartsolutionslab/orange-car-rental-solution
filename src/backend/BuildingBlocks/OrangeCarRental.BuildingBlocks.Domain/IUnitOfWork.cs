namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Unit of Work interface for managing transactional boundaries.
///     Provides a single point to commit all changes made during a business operation.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Saves all changes made in the current unit of work to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
