using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence;

public sealed class LocationRepository(LocationsDbContext context) : ILocationRepository
{
    public async Task<Domain.Location.Location> GetByIdAsync(
        LocationIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        return location ?? throw new EntityNotFoundException(typeof(Domain.Location.Location), id);
    }

    public async Task<Domain.Location.Location?> GetByCodeAsync(
        LocationCode code,
        CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);
    }

    public async Task<List<Domain.Location.Location>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Locations
            .Where(l => l.Status == LocationStatus.Active)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Domain.Location.Location location,
        CancellationToken cancellationToken = default)
    {
        await context.Locations.AddAsync(location, cancellationToken);
    }

    public Task UpdateAsync(
        Domain.Location.Location location,
        CancellationToken cancellationToken = default)
    {
        context.Locations.Update(location);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        Domain.Location.Location location,
        CancellationToken cancellationToken = default)
    {
        context.Locations.Remove(location);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
