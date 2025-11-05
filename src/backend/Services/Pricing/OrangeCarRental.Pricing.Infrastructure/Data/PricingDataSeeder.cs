using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Data;

/// <summary>
///     Seeds the Pricing database with sample pricing policies for development and testing.
///     Creates pricing policies for all German vehicle categories with German VAT (19%).
/// </summary>
public class PricingDataSeeder(
    PricingDbContext context,
    ILogger<PricingDataSeeder> logger)
{
    /// <summary>
    ///     Seeds the database with sample pricing policies if no policies exist.
    /// </summary>
    public async Task SeedAsync()
    {
        // Check if data already exists
        var existingCount = await context.PricingPolicies.CountAsync();
        if (existingCount > 0)
        {
            logger.LogInformation("Pricing database already contains {Count} pricing policies. Skipping seed.",
                existingCount);
            return;
        }

        logger.LogInformation("Seeding Pricing database with sample pricing policies...");

        var policies = CreateSamplePricingPolicies();
        await context.PricingPolicies.AddRangeAsync(policies);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} pricing policies.", policies.Count);
    }

    private static List<PricingPolicy> CreateSamplePricingPolicies()
    {
        var policies = new List<PricingPolicy>();

        // German vehicle category pricing (all prices are net amounts, 19% VAT will be added)
        // Pricing based on typical German car rental rates

        // Kleinwagen (Small Car) - €29.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("KLEIN"),
            Money.Euro(29.99m),
            DateTime.UtcNow,
            null));

        // Kompaktklasse (Compact) - €39.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("KOMPAKT"),
            Money.Euro(39.99m),
            DateTime.UtcNow,
            null));

        // Mittelklasse (Mid-size) - €54.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("MITTEL"),
            Money.Euro(54.99m),
            DateTime.UtcNow,
            null));

        // Oberklasse (Upper Class) - €89.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("OBER"),
            Money.Euro(89.99m),
            DateTime.UtcNow,
            null));

        // SUV - €69.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("SUV"),
            Money.Euro(69.99m),
            DateTime.UtcNow,
            null));

        // Kombi (Station Wagon) - €49.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("KOMBI"),
            Money.Euro(49.99m),
            DateTime.UtcNow,
            null));

        // Transporter (Van) - €79.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("TRANS"),
            Money.Euro(79.99m),
            DateTime.UtcNow,
            null));

        // Luxusklasse (Luxury) - €149.99/day net
        policies.Add(PricingPolicy.Create(
            CategoryCode.Of("LUXUS"),
            Money.Euro(149.99m),
            DateTime.UtcNow,
            null));

        return policies;
    }
}
