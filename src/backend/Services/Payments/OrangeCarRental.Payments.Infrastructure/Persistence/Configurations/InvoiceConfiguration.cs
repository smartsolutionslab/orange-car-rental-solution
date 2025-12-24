using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence.Configurations;

/// <summary>
///     EF Core configuration for Invoice entity.
/// </summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => InvoiceIdentifier.From(value))
            .HasColumnName("Id");

        // Invoice number (unique index)
        builder.Property(x => x.InvoiceNumber)
            .HasConversion(
                num => num.Value,
                value => InvoiceNumber.Parse(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_Invoices_InvoiceNumber");

        // Dates
        builder.Property(x => x.InvoiceDate).IsRequired();
        builder.Property(x => x.ServiceDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();

        // Status
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Seller information
        builder.Property(x => x.SellerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SellerStreet).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SellerPostalCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.SellerCity).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SellerCountry).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SellerEmail).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SellerPhone).HasMaxLength(50).IsRequired();
        builder.Property(x => x.TradeRegisterNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.VatId).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TaxNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ManagingDirector).HasMaxLength(200).IsRequired();

        // Customer information
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CustomerStreet).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CustomerPostalCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.CustomerCity).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerCountry).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerVatId).HasMaxLength(20);

        // References
        builder.Property(x => x.ReservationId).IsRequired();
        builder.Property(x => x.PaymentId);

        // Currency
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();

        // Timestamps
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.SentAt);
        builder.Property(x => x.PaidAt);
        builder.Property(x => x.VoidedAt);

        // PDF document (large binary)
        builder.Property(x => x.PdfDocument)
            .HasColumnType("varbinary(max)");

        // Line items stored as JSON
        builder.OwnsMany(x => x.LineItems, lineItemBuilder =>
        {
            lineItemBuilder.ToJson("LineItems");
            lineItemBuilder.Property(li => li.Position);
            lineItemBuilder.Property(li => li.Description).HasMaxLength(500);
            lineItemBuilder.Property(li => li.Quantity);
            lineItemBuilder.Property(li => li.Unit).HasMaxLength(20);
            lineItemBuilder.Property(li => li.UnitPriceNet).HasPrecision(18, 2);
            lineItemBuilder.Property(li => li.VatRate).HasPrecision(5, 4);
            lineItemBuilder.Property(li => li.ServicePeriodStart);
            lineItemBuilder.Property(li => li.ServicePeriodEnd);
        });

        // Indexes for common queries
        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_Invoices_CustomerId");

        builder.HasIndex(x => x.ReservationId)
            .HasDatabaseName("IX_Invoices_ReservationId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Invoices_Status");

        builder.HasIndex(x => x.InvoiceDate)
            .HasDatabaseName("IX_Invoices_InvoiceDate");

        // Ignore computed properties
        builder.Ignore(x => x.TotalNet);
        builder.Ignore(x => x.TotalVat);
        builder.Ignore(x => x.TotalGross);
    }
}
