using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        // Primary key
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("PaymentId")
            .HasConversion(
                id => id.Value,
                value => PaymentIdentifier.From(value))
            .IsRequired();

        // Foreign keys (value objects)
        builder.Property(p => p.ReservationId)
            .HasColumnName("ReservationId")
            .HasConversion(
                id => id.Value,
                value => ReservationId.From(value))
            .IsRequired();

        builder.Property(p => p.CustomerId)
            .HasColumnName("CustomerId")
            .HasConversion(
                id => id.Value,
                value => CustomerId.From(value))
            .IsRequired();

        // Money value object - complex type mapping
        builder.ComplexProperty(p => p.Amount, money =>
        {
            money.Property(m => m.NetAmount)
                .HasColumnName("AmountNet")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.VatAmount)
                .HasColumnName("AmountVat")
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

        // Enums
        builder.Property(p => p.Method)
            .HasColumnName("PaymentMethod")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Nullable strings
        builder.Property(p => p.TransactionId)
            .HasColumnName("TransactionId")
            .HasMaxLength(200);

        builder.Property(p => p.ErrorMessage)
            .HasColumnName("ErrorMessage")
            .HasMaxLength(1000);

        // Timestamps
        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(p => p.ProcessedAt)
            .HasColumnName("ProcessedAt");

        builder.Property(p => p.RefundedAt)
            .HasColumnName("RefundedAt");

        // Ignore domain events
        builder.Ignore(p => p.DomainEvents);

        // Indexes
        builder.HasIndex(p => p.ReservationId);
        builder.HasIndex(p => p.CustomerId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TransactionId);
    }
}
