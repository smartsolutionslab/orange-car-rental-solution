using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Repository interface for Location aggregate.
///     Provides access to locations in the persistence layer.
/// </summary>
public interface ILocationRepository : IRepository<Location, LocationIdentifier>
{
    // All common repository methods (GetByIdAsync, AddAsync, UpdateAsync, RemoveAsync, SaveChangesAsync)
    // are inherited from IRepository<Location, LocationIdentifier>

    /// <summary>
    ///     Gets a location by its code.
    /// </summary>
    Task<Location?> GetByCodeAsync(LocationCode code, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all active locations.
    /// </summary>
    Task<List<Location>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
