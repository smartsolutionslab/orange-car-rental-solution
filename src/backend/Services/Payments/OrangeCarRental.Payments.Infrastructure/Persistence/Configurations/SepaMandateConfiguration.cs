using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence.Configurations;

internal sealed class SepaMandateConfiguration : IEntityTypeConfiguration<SepaMandate>
{
    public void Configure(EntityTypeBuilder<SepaMandate> builder)
    {
        builder.ToTable("SepaMandates");

        // Primary key
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("SepaMandateId")
            .HasConversion(
                id => id.Value,
                value => SepaMandateIdentifier.From(value))
            .IsRequired();

        // Mandate Reference value object
        builder.Property(m => m.MandateReference)
            .HasColumnName("MandateReference")
            .HasConversion(
                mr => mr.Value,
                value => MandateReference.Parse(value))
            .HasMaxLength(35)
            .IsRequired();

        // IBAN value object
        builder.Property(m => m.IBAN)
            .HasColumnName("IBAN")
            .HasConversion(
                iban => iban.Value,
                value => IBAN.Create(value))
            .HasMaxLength(34)
            .IsRequired();

        // BIC value object
        builder.Property(m => m.BIC)
            .HasColumnName("BIC")
            .HasConversion(
                bic => bic.Value,
                value => BIC.Create(value))
            .HasMaxLength(11)
            .IsRequired();

        // Account holder
        builder.Property(m => m.AccountHolder)
            .HasColumnName("AccountHolder")
            .HasMaxLength(140) // SEPA max name length
            .IsRequired();

        // Status enum
        builder.Property(m => m.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Customer reference (value object)
        builder.Property(m => m.CustomerId)
            .HasColumnName("CustomerId")
            .HasConversion(
                id => id.Value,
                value => CustomerId.From(value))
            .IsRequired();

        // Timestamps
        builder.Property(m => m.SignedAt)
            .HasColumnName("SignedAt")
            .IsRequired();

        builder.Property(m => m.RevokedAt)
            .HasColumnName("RevokedAt");

        builder.Property(m => m.LastUsedAt)
            .HasColumnName("LastUsedAt");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        // Ignore domain events
        builder.Ignore(m => m.DomainEvents);

        // Indexes
        builder.HasIndex(m => m.CustomerId);
        builder.HasIndex(m => m.MandateReference).IsUnique();
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => new { m.CustomerId, m.Status });
    }
}
