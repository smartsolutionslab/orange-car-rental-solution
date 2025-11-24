using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence.Configurations;

/// <summary>
///     Entity configuration for Location aggregate.
/// </summary>
internal sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        // Primary key - LocationIdentifier struct with Guid value
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasColumnName("LocationId")
            .HasConversion(
                id => id.Value,
                value => LocationIdentifier.From(value))
            .IsRequired();

        // LocationCode value object - unique identifier
        builder.Property(l => l.Code)
            .HasColumnName("Code")
            .HasConversion(
                code => code.Value,
                value => LocationCode.Of(value))
            .HasMaxLength(20)
            .IsRequired();

        // LocationName value object
        builder.Property(l => l.Name)
            .HasColumnName("Name")
            .HasConversion(
                name => name.Value,
                value => LocationName.Of(value))
            .HasMaxLength(100)
            .IsRequired();

        // Address value object - complex type mapping
        builder.ComplexProperty(l => l.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasConversion(
                    street => street.Value,
                    value => Street.Of(value))
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasConversion(
                    city => city.Value,
                    value => City.Of(value))
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasConversion(
                    postalCode => postalCode.Value,
                    value => PostalCode.Of(value))
                .HasMaxLength(20)
                .IsRequired();
        });

        // LocationStatus enum
        builder.Property(l => l.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Ignore domain events (not persisted)
        builder.Ignore(l => l.DomainEvents);

        // Indexes for common queries
        builder.HasIndex(l => l.Code)
            .IsUnique(); // Location code must be unique

        builder.HasIndex(l => l.Status);
    }
}
