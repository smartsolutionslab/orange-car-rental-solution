using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence;

/// <summary>
///     Database context for the Pricing service.
///     Manages pricing policies and rate calculations.
///     Implements IUnitOfWork for transaction management.
/// </summary>
public sealed class PricingDbContext : DbContext, IUnitOfWork
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options)
    {
    }

    public DbSet<PricingPolicy> PricingPolicies => Set<PricingPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PricingPolicyConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("pricing");
    }
}
