using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
///     Database context for the Reservations service.
///     Manages reservation data and booking operations.
///     Implements IUnitOfWork for transaction management.
/// </summary>
public sealed class ReservationsDbContext : DbContext, IUnitOfWork
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {
    }

    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ReservationConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("reservations");
    }
}
