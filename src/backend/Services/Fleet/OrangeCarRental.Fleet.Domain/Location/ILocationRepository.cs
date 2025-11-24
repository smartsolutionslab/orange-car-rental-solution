using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Repository interface for Location aggregate.
/// </summary>
public interface ILocationRepository
{
    /// <summary>
    ///     Gets a location by its identifier.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the location is not found.</exception>
    Task<Location> GetByIdAsync(LocationIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a location by its code.
    /// </summary>
    /// <param name="code">The location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location if found, otherwise null.</returns>
    Task<Location?> GetByCodeAsync(LocationCode code, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all locations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all locations.</returns>
    Task<List<Location>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all active locations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active locations.</returns>
    Task<List<Location>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a location with the given code exists.
    /// </summary>
    /// <param name="code">The location code to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if location exists, otherwise false.</returns>
    Task<bool> ExistsWithCodeAsync(LocationCode code, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new location.
    /// </summary>
    /// <param name="location">The location to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Location location, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing location.
    /// </summary>
    /// <param name="location">The location to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a location.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(LocationIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves all pending changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
