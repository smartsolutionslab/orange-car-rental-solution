using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence.Configurations;

/// <summary>
///     EF Core configuration for Invoice entity.
/// </summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

        // Seller information (owned entity)
        builder.OwnsOne(x => x.Seller, sellerBuilder =>
        {
            sellerBuilder.Property(s => s.CompanyName).HasColumnName("SellerName").HasMaxLength(200).IsRequired();
            sellerBuilder.Property(s => s.Street).HasColumnName("SellerStreet").HasMaxLength(200).IsRequired();
            sellerBuilder.Property(s => s.PostalCode).HasColumnName("SellerPostalCode").HasMaxLength(10).IsRequired();
            sellerBuilder.Property(s => s.City).HasColumnName("SellerCity").HasMaxLength(100).IsRequired();
            sellerBuilder.Property(s => s.Country).HasColumnName("SellerCountry").HasMaxLength(100).IsRequired();
            sellerBuilder.Property(s => s.Email).HasColumnName("SellerEmail").HasMaxLength(200).IsRequired();
            sellerBuilder.Property(s => s.Phone).HasColumnName("SellerPhone").HasMaxLength(50).IsRequired();
            sellerBuilder.Property(s => s.TradeRegisterNumber).HasColumnName("TradeRegisterNumber").HasMaxLength(50).IsRequired();
            sellerBuilder.Property(s => s.VatId).HasColumnName("VatId").HasMaxLength(20).IsRequired();
            sellerBuilder.Property(s => s.TaxNumber).HasColumnName("TaxNumber").HasMaxLength(30).IsRequired();
            sellerBuilder.Property(s => s.ManagingDirector)
                .HasColumnName("ManagingDirector")
                .HasMaxLength(200)
                .IsRequired()
                .HasConversion(
                    name => name.Value,
                    value => PersonName.Of(value));
        });

        // Customer information (owned entity)
        builder.OwnsOne(x => x.Customer, customerBuilder =>
        {
            customerBuilder.Property(c => c.CustomerId).HasColumnName("CustomerId").IsRequired();
            customerBuilder.Property(c => c.Name).HasColumnName("CustomerName").HasMaxLength(200).IsRequired();
            customerBuilder.Property(c => c.Street).HasColumnName("CustomerStreet").HasMaxLength(200).IsRequired();
            customerBuilder.Property(c => c.PostalCode).HasColumnName("CustomerPostalCode").HasMaxLength(10).IsRequired();
            customerBuilder.Property(c => c.City).HasColumnName("CustomerCity").HasMaxLength(100).IsRequired();
            customerBuilder.Property(c => c.Country).HasColumnName("CustomerCountry").HasMaxLength(100).IsRequired();
            customerBuilder.Property(c => c.VatId).HasColumnName("CustomerVatId").HasMaxLength(20);

            // Index on CustomerId
            customerBuilder.HasIndex(c => c.CustomerId)
                .HasDatabaseName("IX_Invoices_CustomerId");
        });

        // References
        builder.Property(x => x.ReservationId).IsRequired();
        builder.Property(x => x.PaymentId);

        // Currency (value object conversion)
        builder.Property(x => x.Currency)
            .HasColumnName("CurrencyCode")
            .HasMaxLength(3)
            .IsRequired()
            .HasConversion(
                currency => currency.Code,
                code => Currency.From(code));

        // Timestamps
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.SentAt);
        builder.Property(x => x.PaidAt);
        builder.Property(x => x.VoidedAt);

        // PDF document (large binary)
        builder.Property(x => x.PdfDocument)
            .HasColumnType("varbinary(max)");

        // Line items stored as JSON with custom serialization
        builder.Property(x => x.LineItems)
            .HasColumnName("LineItems")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                items => SerializeLineItems(items),
                json => DeserializeLineItems(json))
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<InvoiceLineItem>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        // Indexes for common queries
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

    private static string SerializeLineItems(IReadOnlyList<InvoiceLineItem> items)
    {
        var dtos = items.Select(item => new LineItemDto
        {
            Position = item.Position,
            Description = item.Description,
            QuantityValue = item.Quantity.Value,
            QuantityUnit = item.Quantity.Unit,
            UnitPriceNet = item.UnitPrice.NetAmount,
            UnitPriceCurrency = item.UnitPrice.Currency.Code,
            VatRate = item.VatRate.Value,
            ServicePeriodStart = item.ServicePeriodStart,
            ServicePeriodEnd = item.ServicePeriodEnd
        }).ToList();

        return JsonSerializer.Serialize(dtos, JsonOptions);
    }

    private static IReadOnlyList<InvoiceLineItem> DeserializeLineItems(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return Array.Empty<InvoiceLineItem>();

        var dtos = JsonSerializer.Deserialize<List<LineItemDto>>(json, JsonOptions)
            ?? new List<LineItemDto>();

        return dtos.Select(dto => new InvoiceLineItem
        {
            Position = dto.Position,
            Description = dto.Description,
            Quantity = Quantity.Of(dto.QuantityValue, dto.QuantityUnit),
            UnitPrice = Money.Of(dto.UnitPriceNet, dto.VatRate, Currency.From(dto.UnitPriceCurrency)),
            VatRate = VatRate.Of(dto.VatRate),
            ServicePeriodStart = dto.ServicePeriodStart,
            ServicePeriodEnd = dto.ServicePeriodEnd
        }).ToList();
    }

    private sealed class LineItemDto
    {
        public int Position { get; set; }
        public string Description { get; set; } = string.Empty;
        public int QuantityValue { get; set; }
        public string QuantityUnit { get; set; } = string.Empty;
        public decimal UnitPriceNet { get; set; }
        public string UnitPriceCurrency { get; set; } = "EUR";
        public decimal VatRate { get; set; }
        public DateOnly? ServicePeriodStart { get; set; }
        public DateOnly? ServicePeriodEnd { get; set; }
    }
}
