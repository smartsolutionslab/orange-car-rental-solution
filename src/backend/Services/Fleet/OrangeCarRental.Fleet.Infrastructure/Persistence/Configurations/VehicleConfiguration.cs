using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence.Configurations;

/// <summary>
///     Entity configuration for Vehicle aggregate.
/// </summary>
internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        // Primary key - VehicleIdentifier struct with Guid value
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("VehicleId")
            .HasConversion(
                id => id.Value,
                value => VehicleIdentifier.From(value))
            .IsRequired();

        // VehicleName value object
        builder.Property(v => v.Name)
            .HasColumnName("Name")
            .HasConversion(
                name => name.Value,
                value => VehicleName.From(value))
            .HasMaxLength(100)
            .IsRequired();

        // VehicleCategory - stored as code
        builder.Property(v => v.Category)
            .HasColumnName("CategoryCode")
            .HasConversion(
                category => category.Code,
                code => VehicleCategory.From(code))
            .HasMaxLength(20)
            .IsRequired();

        // LocationCode - stored as string
        builder.Property(v => v.CurrentLocationCode)
            .HasColumnName("LocationCode")
            .HasConversion(
                code => code.Value,
                value => LocationCode.From(value))
            .HasMaxLength(20)
            .IsRequired();

        // Money value object - complex type mapping for German VAT
        builder.ComplexProperty(v => v.DailyRate, money =>
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
                    code => Currency.From(code))
                .HasMaxLength(3)
                .IsRequired();
        });

        // SeatingCapacity value object
        builder.Property(v => v.Seats)
            .HasColumnName("Seats")
            .HasConversion(
                seats => seats.Value,
                value => SeatingCapacity.From(value))
            .IsRequired();

        // Enums
        builder.Property(v => v.FuelType)
            .HasColumnName("FuelType")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(v => v.TransmissionType)
            .HasColumnName("TransmissionType")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(v => v.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Simple properties
        builder.Property(v => v.LicensePlate)
            .HasColumnName("LicensePlate")
            .HasMaxLength(20);

        // Nullable value objects - Manufacturer, Model, Year
        builder.Property(v => v.Manufacturer)
            .HasColumnName("Manufacturer")
            .HasConversion(
                manufacturer => manufacturer.HasValue ? manufacturer.Value.Value : null,
                value => value != null ? Manufacturer.From(value) : null)
            .HasMaxLength(100);

        builder.Property(v => v.Model)
            .HasColumnName("Model")
            .HasConversion(
                model => model.HasValue ? model.Value.Value : null,
                value => value != null ? VehicleModel.From(value) : null)
            .HasMaxLength(100);

        builder.Property(v => v.Year)
            .HasColumnName("Year")
            .HasConversion(
                year => year.HasValue ? year.Value.Value : (int?)null,
                value => value.HasValue ? ManufacturingYear.From(value.Value) : null);

        builder.Property(v => v.ImageUrl)
            .HasColumnName("ImageUrl")
            .HasMaxLength(500);

        // Ignore domain events (not persisted)
        builder.Ignore(v => v.DomainEvents);

        // Indexes for common queries
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.CurrentLocationCode);
        builder.HasIndex(v => v.Category);
    }
}
