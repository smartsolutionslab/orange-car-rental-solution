using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence.Configurations;

internal sealed class LocationConfiguration : IEntityTypeConfiguration<Domain.Location.Location>
{
    public void Configure(EntityTypeBuilder<Domain.Location.Location> builder)
    {
        builder.ToTable("Locations");

        // Primary key - LocationCode (natural key)
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasColumnName("Code")
            .HasConversion(
                code => code.Value,
                value => LocationCode.From(value))
            .HasMaxLength(20)
            .IsRequired();

        // Code is an alias for Id, so we ignore it to avoid duplicate mapping
        builder.Ignore(l => l.Code);

        // LocationName value object
        builder.Property(l => l.Name)
            .HasColumnName("Name")
            .HasConversion(
                name => name.Value,
                value => LocationName.From(value))
            .HasMaxLength(100)
            .IsRequired();

        // LocationAddress value object - complex type
        builder.ComplexProperty(l => l.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(10)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .HasMaxLength(100)
                .IsRequired();
        });

        // GeoCoordinates value object (nullable) - ignore for now (can add later as owned entity)
        builder.Ignore(l => l.Coordinates);

        // OpeningHours value object
        builder.Property(l => l.OpeningHours)
            .HasColumnName("OpeningHours")
            .HasConversion(
                hours => hours.Value,
                value => OpeningHours.From(value))
            .HasMaxLength(500)
            .IsRequired();

        // ContactInfo value object - complex type
        builder.ComplexProperty(l => l.Contact, contact =>
        {
            contact.Property(c => c.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(20)
                .IsRequired();

            contact.Property(c => c.Email)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
        });

        // LocationStatus enum
        builder.Property(l => l.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Timestamps
        builder.Property(l => l.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("UpdatedAt");

        // Ignore domain events
        builder.Ignore(l => l.DomainEvents);

        // Index for status queries
        builder.HasIndex(l => l.Status);
    }
}
