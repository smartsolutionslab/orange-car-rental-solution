using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

/// <summary>
///     Database context for the Payments service.
///     Implements IUnitOfWork for transaction management.
/// </summary>
public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("payments");
    }
}
