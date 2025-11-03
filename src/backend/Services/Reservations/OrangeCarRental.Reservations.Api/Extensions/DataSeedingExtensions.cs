using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Extensions;

/// <summary>
/// Extension methods for data seeding in the Reservations API.
/// </summary>
public static class DataSeedingExtensions
{
    /// <summary>
    /// Seeds the Reservations database with sample data if running in Development environment.
    /// </summary>
    public static async Task SeedReservationsDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();

        // Only seed in development
        if (!environment.IsDevelopment())
        {
            return;
        }

        var logger = services.GetRequiredService<ILogger<ReservationsDataSeeder>>();

        try
        {
            var context = services.GetRequiredService<ReservationsDbContext>();

            var seeder = new ReservationsDataSeeder(context, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the Reservations database.");
            // Don't throw - seeding failures shouldn't prevent the app from starting
        }
    }
}
