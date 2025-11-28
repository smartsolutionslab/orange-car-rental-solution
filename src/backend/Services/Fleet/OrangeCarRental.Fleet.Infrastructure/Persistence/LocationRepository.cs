using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of ILocationRepository.
/// </summary>
public sealed class LocationRepository(FleetDbContext context) : ILocationRepository
{
    public async Task<Location> GetByCodeAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == code, cancellationToken);

        return location ?? throw new EntityNotFoundException(typeof(Location), code.Value);
    }

    public async Task<Location?> FindByCodeAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .FirstOrDefaultAsync(l => l.Id == code, cancellationToken);
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

    public async Task<bool> ExistsAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .AnyAsync(l => l.Id == code, cancellationToken);
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

    public async Task DeleteAsync(LocationCode code, CancellationToken cancellationToken = default)
    {
        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == code, cancellationToken);

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
