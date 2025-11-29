using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Database context for the Fleet service.
///     Manages vehicle data and fleet operations.
///     Implements IUnitOfWork for transaction management.
/// </summary>
public sealed class FleetDbContext(DbContextOptions<FleetDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("fleet");
    }
}
