using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence.Configurations;

/// <summary>
///     Entity configuration for PricingPolicy aggregate.
/// </summary>
internal sealed class PricingPolicyConfiguration : IEntityTypeConfiguration<PricingPolicy>
{
    public void Configure(EntityTypeBuilder<PricingPolicy> builder)
    {
        builder.ToTable("PricingPolicies");

        // Primary key - PricingPolicyId struct with Guid value
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("PricingPolicyId")
            .HasConversion(
                id => id.Value,
                value => PricingPolicyIdentifier.From(value))
            .IsRequired();

        // CategoryCode value object
        builder.Property(p => p.CategoryCode)
            .HasColumnName("CategoryCode")
            .HasConversion(
                code => code.Value,
                value => CategoryCode.Of(value))
            .HasMaxLength(20)
            .IsRequired();

        // LocationCode value object (optional for location-specific pricing)
        builder.Property(p => p.LocationCode)
            .HasColumnName("LocationCode")
            .HasConversion(
                code => code!.Value.Value,
                value => LocationCode.Of(value))
            .HasMaxLength(20)
            .IsRequired(false);

        // Money value object - complex type mapping for German VAT
        builder.ComplexProperty(p => p.DailyRate, money =>
        {
            money.Property(m => m.NetAmount)
                .HasColumnName("DailyRateNet")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.VatAmount)
                .HasColumnName("DailyRateVat")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.Of(code))
                .HasMaxLength(3)
                .IsRequired();
        });

        // IsActive flag
        builder.Property(p => p.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();

        // Effective dates
        builder.Property(p => p.EffectiveFrom)
            .HasColumnName("EffectiveFrom")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(p => p.EffectiveUntil)
            .HasColumnName("EffectiveUntil")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // Indexes for query performance
        builder.HasIndex(p => p.CategoryCode)
            .HasDatabaseName("IX_PricingPolicies_CategoryCode");

        builder.HasIndex(p => new { p.CategoryCode, p.IsActive })
            .HasDatabaseName("IX_PricingPolicies_CategoryCode_IsActive");

        builder.HasIndex(p => new { p.CategoryCode, p.LocationCode, p.IsActive })
            .HasDatabaseName("IX_PricingPolicies_CategoryCode_LocationCode_IsActive");

        // Ignore domain events (not persisted)
        builder.Ignore(p => p.DomainEvents);
    }
}
