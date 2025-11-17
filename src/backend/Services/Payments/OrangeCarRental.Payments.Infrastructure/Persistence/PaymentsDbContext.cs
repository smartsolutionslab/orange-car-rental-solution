using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PaymentConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("payments");
    }
}
