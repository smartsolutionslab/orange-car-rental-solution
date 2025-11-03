using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Data;

/// <summary>
/// Seeds the Reservations database with sample reservations for development and testing.
/// Creates sample reservations with future booking periods.
/// </summary>
public class ReservationsDataSeeder(
    ReservationsDbContext context,
    ILogger<ReservationsDataSeeder> logger)
{

    /// <summary>
    /// Seeds the database with sample reservations if no reservations exist.
    /// Note: Requires vehicles to exist in the Fleet database.
    /// </summary>
    public async Task SeedAsync()
    {
        // Check if data already exists
        var existingCount = await context.Reservations.CountAsync();
        if (existingCount > 0)
        {
            logger.LogInformation("Reservations database already contains {Count} reservations. Skipping seed.", existingCount);
            return;
        }

        logger.LogInformation("Seeding Reservations database with sample reservations...");

        var reservations = CreateSampleReservations();
        await context.Reservations.AddRangeAsync(reservations);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} reservations to Reservations database.", reservations.Count);
    }

    private List<Reservation> CreateSampleReservations()
    {
        var reservations = new List<Reservation>();

        // Sample customer IDs (in a real system, these would come from a Customer service)
        var customer1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var customer2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var customer3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Sample vehicle IDs (these should match vehicles in the Fleet database)
        // Note: In production, you would query the Fleet database to get actual vehicle IDs
        var vehicle1 = Guid.NewGuid(); // Would be actual vehicle ID
        var vehicle2 = Guid.NewGuid();
        var vehicle3 = Guid.NewGuid();

        var today = DateTime.UtcNow.Date;

        // Future reservations (Confirmed)
        reservations.Add(CreateReservation(
            vehicle1,
            customer1,
            today.AddDays(5),
            today.AddDays(8),
            Money.Euro(49.99m * 3) // 3 days at 49.99/day
        ));

        reservations.Add(CreateReservation(
            vehicle2,
            customer2,
            today.AddDays(10),
            today.AddDays(17),
            Money.Euro(89.99m * 7) // 7 days at 89.99/day
        ));

        // Pending reservations
        var pendingReservation = CreateReservation(
            vehicle3,
            customer3,
            today.AddDays(15),
            today.AddDays(20),
            Money.Euro(119.99m * 5) // 5 days at 119.99/day
        );
        reservations.Add(pendingReservation);

        // Confirm first two reservations
        reservations[0].Confirm();
        reservations[1].Confirm();

        return reservations;
    }

    private Reservation CreateReservation(
        Guid vehicleId,
        Guid customerId,
        DateTime pickupDate,
        DateTime returnDate,
        Money totalPrice,
        string pickupLocationCode = "BER-HBF",
        string dropoffLocationCode = "BER-HBF")
    {
        var period = BookingPeriod.Of(pickupDate, returnDate);
        return Reservation.Create(
            vehicleId,
            customerId,
            period,
            LocationCode.Of(pickupLocationCode),
            LocationCode.Of(dropoffLocationCode),
            totalPrice);
    }
}
