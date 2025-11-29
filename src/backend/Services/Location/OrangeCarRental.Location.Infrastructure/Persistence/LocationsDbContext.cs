using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence;

/// <summary>
///     Database context for the Location service.
///     Implements IUnitOfWork for transaction management.
/// </summary>
public sealed class LocationsDbContext(DbContextOptions<LocationsDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Domain.Location.Location> Locations => Set<Domain.Location.Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new LocationConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("locations");
    }
}
