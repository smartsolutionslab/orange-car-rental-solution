using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Repository interface for Location aggregate.
///     Provides access to locations in the persistence layer.
///     Uses LocationCode as the identity/key.
/// </summary>
public interface ILocationRepository : IRepository<Location, LocationCode>
{
    // All common repository methods (GetByIdAsync, AddAsync, UpdateAsync, RemoveAsync, SaveChangesAsync)
    // are inherited from IRepository<Location, LocationCode>
    // Note: GetByIdAsync now takes LocationCode as the identifier

    /// <summary>
    ///     Tries to get a location by its code. Returns null if not found.
    /// </summary>
    Task<Location?> FindByCodeAsync(LocationCode code, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all active locations.
    /// </summary>
    Task<List<Location>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
