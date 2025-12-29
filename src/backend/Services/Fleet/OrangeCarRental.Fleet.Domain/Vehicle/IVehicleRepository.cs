using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Repository interface for Vehicle aggregate.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    ///     Gets a vehicle by its identifier.
    /// </summary>
    /// <param name="id">The vehicle identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The vehicle.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when the vehicle is not found.</exception>
    Task<Vehicle> GetByIdAsync(
        VehicleIdentifier id,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all vehicles (use with caution - prefer SearchAsync for large datasets).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An immutable read-only list of all vehicles.</returns>
    Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Streams all vehicles asynchronously without loading the entire collection into memory.
    ///     Preferred over GetAllAsync for large datasets, batch processing, or streaming responses.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable stream of vehicles.</returns>
    IAsyncEnumerable<Vehicle> StreamAllAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<Vehicle>> SearchAsync(
        VehicleSearchParameters parameters,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        VehicleIdentifier id,
        CancellationToken cancellationToken = default);
}
