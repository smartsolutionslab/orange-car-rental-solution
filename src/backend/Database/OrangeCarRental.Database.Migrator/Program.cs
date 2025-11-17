using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

// Add logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register all DbContexts
builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("fleet") ??
                         throw new InvalidOperationException("Fleet connection string is missing")));

builder.Services.AddDbContext<ReservationsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("reservations") ??
                         throw new InvalidOperationException("Reservations connection string is missing")));

builder.Services.AddDbContext<CustomersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("customers") ??
                         throw new InvalidOperationException("Customers connection string is missing")));

builder.Services.AddDbContext<PricingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("pricing") ??
                         throw new InvalidOperationException("Pricing connection string is missing")));

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("=== Orange Car Rental - Database Migrator ===");
logger.LogInformation("Starting database migrations...");

try
{
    // Apply Fleet migrations
    logger.LogInformation("Migrating Fleet database...");
    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FleetDbContext>();
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("✓ Fleet database migrated successfully");
    }

    // Apply Reservations migrations
    logger.LogInformation("Migrating Reservations database...");
    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("✓ Reservations database migrated successfully");
    }

    // Apply Customers migrations
    logger.LogInformation("Migrating Customers database...");
    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("✓ Customers database migrated successfully");
    }

    // Apply Pricing migrations
    logger.LogInformation("Migrating Pricing database...");
    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("✓ Pricing database migrated successfully");
    }

    logger.LogInformation("===========================================");
    logger.LogInformation("All database migrations completed successfully!");
    logger.LogInformation("===========================================");

    Environment.ExitCode = 0;
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while migrating databases");
    logger.LogError("Migration failed. Please check the error message above.");
    Environment.ExitCode = 1;
}
