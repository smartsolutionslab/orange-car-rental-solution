using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of ILocationRepository.
/// </summary>
public sealed class LocationRepository(FleetDbContext context) : ILocationRepository
{
    private DbSet<Location> Locations => context.Locations;
    public async Task<Location> GetByCodeAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        var location = await Locations.FirstOrDefaultAsync(l => l.Id == code, cancellationToken);

        return location ?? throw new EntityNotFoundException(typeof(Location), code.Value);
    }

    public async Task<Location?> FindByCodeAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        return await Locations.FirstOrDefaultAsync(l => l.Id == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Locations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public IAsyncEnumerable<Location> StreamAllAsync(CancellationToken cancellationToken = default)
    {
        return Locations
            .AsNoTracking()
            .AsAsyncEnumerable();
    }

    public async Task<IReadOnlyList<Location>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Locations
            .AsNoTracking()
            .Where(l => l.Status == LocationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public IAsyncEnumerable<Location> StreamAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return Locations
            .AsNoTracking()
            .Where(l => l.Status == LocationStatus.Active)
            .AsAsyncEnumerable();
    }

    public async Task<bool> ExistsAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        return await Locations
            .AnyAsync(l => l.Id == code, cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await Locations.AddAsync(location, cancellationToken);
    }

    public Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        Locations.Update(location);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        var location = await Locations.FirstOrDefaultAsync(l => l.Id == code, cancellationToken);

        if (location != null)
        {
            context.Locations.Remove(location);
        }
    }
}
