using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of ILocationRepository.
/// </summary>
public sealed class LocationRepository(FleetDbContext context) : ILocationRepository
{
    public async Task<Location> GetByIdAsync(LocationIdentifier id, CancellationToken cancellationToken = default)
    {
        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        return location ?? throw new EntityNotFoundException(typeof(Location), id);
    }

    public async Task<Location?> GetByCodeAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);
    }

    public async Task<List<Location>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Location>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .AsNoTracking()
            .Where(l => l.Status == LocationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithCodeAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .AnyAsync(l => l.Code == code, cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await context.Locations.AddAsync(location, cancellationToken);
    }

    public Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        context.Locations.Update(location);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(LocationIdentifier id, CancellationToken cancellationToken = default)
    {
        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (location != null)
        {
            context.Locations.Remove(location);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
