using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Repository interface for Location aggregate.
/// </summary>
public interface ILocationRepository
{
    /// <summary>
    ///     Gets a location by its code (the natural key/identity).
    /// </summary>
    /// <param name="code">The location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the location is not found.</exception>
    Task<Location> GetByCodeAsync(LocationCode code, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Tries to get a location by its code.
    /// </summary>
    /// <param name="code">The location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location if found, otherwise null.</returns>
    Task<Location?> FindByCodeAsync(LocationCode code, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all locations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An immutable read-only list of all locations.</returns>
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all locations. Memory-efficient alternative to GetAllAsync.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of all locations.</returns>
    IAsyncEnumerable<Location> StreamAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all active locations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An immutable read-only list of active locations.</returns>
    Task<IReadOnlyList<Location>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all active locations. Memory-efficient alternative to GetAllActiveAsync.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of active locations.</returns>
    IAsyncEnumerable<Location> StreamAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a location with the given code exists.
    /// </summary>
    /// <param name="code">The location code to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if location exists, otherwise false.</returns>
    Task<bool> ExistsAsync(LocationCode code, CancellationToken cancellationToken = default);

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
    ///     Deletes a location by its code.
    /// </summary>
    /// <param name="code">The location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(LocationCode code, CancellationToken cancellationToken = default);
}
