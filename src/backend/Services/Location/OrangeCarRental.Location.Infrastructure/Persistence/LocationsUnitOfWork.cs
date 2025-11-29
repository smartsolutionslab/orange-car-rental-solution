using SmartSolutionsLab.OrangeCarRental.Location.Domain;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Locations bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class LocationsUnitOfWork : ILocationsUnitOfWork
{
    private readonly LocationsDbContext _context;
    private ILocationRepository? _locations;

    public LocationsUnitOfWork(LocationsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public ILocationRepository Locations =>
        _locations ??= new LocationRepository(_context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
