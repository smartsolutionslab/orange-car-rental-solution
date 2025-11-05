using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Database context for the Fleet service.
///     Manages vehicle data and fleet operations.
/// </summary>
public sealed class FleetDbContext(DbContextOptions<FleetDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new VehicleConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("fleet");
    }
}
