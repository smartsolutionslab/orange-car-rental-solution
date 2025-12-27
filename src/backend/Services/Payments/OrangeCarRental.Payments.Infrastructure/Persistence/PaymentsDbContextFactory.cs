using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        // Design-time connection string for EF Core migrations
        // This is only used for generating migrations, not at runtime
        const string designTimeConnectionString = "Server=localhost;Database=OrangeCarRental_Payments;Trusted_Connection=True;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseSqlServer(designTimeConnectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Payments.Infrastructure"));

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
