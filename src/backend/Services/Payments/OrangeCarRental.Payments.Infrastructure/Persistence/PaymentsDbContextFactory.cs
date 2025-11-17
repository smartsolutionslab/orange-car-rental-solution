using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrangeCarRental.Payments.Api"))
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("PaymentsDatabase");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'PaymentsDatabase' not found in appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Payments.Infrastructure"));

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
