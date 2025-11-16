using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence.Configurations;

/// <summary>
///     Entity configuration for Reservation aggregate.
/// </summary>
internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        // Primary key - ReservationIdentifier struct with Guid value
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("ReservationId")
            .HasConversion(
                id => id.Value,
                value => ReservationIdentifier.From(value))
            .IsRequired();

        // Foreign keys (references to other services) - using internal value objects
        builder.Property(r => r.VehicleId)
            .HasColumnName("VehicleId")
            .HasConversion(
                id => id.Value,
                value => ReservationVehicleId.From(value))
            .IsRequired();

        builder.Property(r => r.CustomerId)
            .HasColumnName("CustomerId")
            .HasConversion(
                id => id.Value,
                value => ReservationCustomerId.From(value))
            .IsRequired();

        // BookingPeriod value object - complex type mapping
        builder.ComplexProperty(r => r.Period, period =>
        {
            period.Property(p => p.PickupDate)
                .HasColumnName("PickupDate")
                .HasColumnType("date")
                .IsRequired();

            period.Property(p => p.ReturnDate)
                .HasColumnName("ReturnDate")
                .HasColumnType("date")
                .IsRequired();
        });

        // LocationCode value objects
        builder.Property(r => r.PickupLocationCode)
            .HasColumnName("PickupLocationCode")
            .HasConversion(
                locationCode => locationCode.Value,
                value => LocationCode.Of(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.DropoffLocationCode)
            .HasColumnName("DropoffLocationCode")
            .HasConversion(
                locationCode => locationCode.Value,
                value => LocationCode.Of(value))
            .HasMaxLength(20)
            .IsRequired();

        // Money value object - complex type mapping for German VAT
        builder.ComplexProperty(r => r.TotalPrice, money =>
        {
            money.Property(m => m.NetAmount)
                .HasColumnName("TotalPriceNet")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.VatAmount)
                .HasColumnName("TotalPriceVat")
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

        // ReservationStatus enum
        builder.Property(r => r.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Simple properties
        builder.Property(r => r.CancellationReason)
            .HasColumnName("CancellationReason")
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(r => r.ConfirmedAt)
            .HasColumnName("ConfirmedAt");

        builder.Property(r => r.CancelledAt)
            .HasColumnName("CancelledAt");

        builder.Property(r => r.CompletedAt)
            .HasColumnName("CompletedAt");

        // Ignore domain events (not persisted)
        builder.Ignore(r => r.DomainEvents);

        // Indexes for common queries
        builder.HasIndex(r => r.VehicleId);
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.PickupLocationCode);
        builder.HasIndex(r => r.DropoffLocationCode);
        builder.HasIndex(r => r.CreatedAt);
    }
}
