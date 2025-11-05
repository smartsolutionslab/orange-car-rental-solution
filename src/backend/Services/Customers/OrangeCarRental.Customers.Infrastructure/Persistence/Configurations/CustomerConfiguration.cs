using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Configurations;

/// <summary>
///     Entity configuration for Customer aggregate.
///     Configures mappings for value objects, complex types, and soft delete.
/// </summary>
internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        // Primary key - CustomerIdentifier struct with Guid value
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("CustomerId")
            .HasConversion(
                id => id.Value,
                value => CustomerIdentifier.From(value))
            .IsRequired();

        // Simple string properties
        builder.Property(c => c.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(100)
            .IsRequired();

        // Email value object - converted to string
        builder.Property(c => c.Email)
            .HasColumnName("Email")
            .HasConversion(
                email => email.Value,
                value => Email.Of(value))
            .HasMaxLength(254)
            .IsRequired();

        // PhoneNumber value object - converted to string
        builder.Property(c => c.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasConversion(
                phone => phone.Value,
                value => PhoneNumber.Of(value))
            .HasMaxLength(20)
            .IsRequired();

        // DateOfBirth
        builder.Property(c => c.DateOfBirth)
            .HasColumnName("DateOfBirth")
            .HasColumnType("date")
            .IsRequired();

        // Address value object - stored as complex type
        builder.ComplexProperty(c => c.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("Address_PostalCode")
                .HasMaxLength(10)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(100)
                .IsRequired();
        });

        // DriversLicense value object - stored as complex type
        builder.ComplexProperty(c => c.DriversLicense, license =>
        {
            license.Property(l => l.LicenseNumber)
                .HasColumnName("DriversLicense_LicenseNumber")
                .HasMaxLength(20)
                .IsRequired();

            license.Property(l => l.IssueCountry)
                .HasColumnName("DriversLicense_IssueCountry")
                .HasMaxLength(100)
                .IsRequired();

            license.Property(l => l.IssueDate)
                .HasColumnName("DriversLicense_IssueDate")
                .HasColumnType("date")
                .IsRequired();

            license.Property(l => l.ExpiryDate)
                .HasColumnName("DriversLicense_ExpiryDate")
                .HasColumnType("date")
                .IsRequired();
        });

        // CustomerStatus enum - stored as string
        builder.Property(c => c.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Timestamps
        builder.Property(c => c.RegisteredAtUtc)
            .HasColumnName("RegisteredAtUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("UpdatedAtUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Indexes for query performance
        builder.HasIndex(c => c.Email)
            .HasDatabaseName("IX_Customers_Email")
            .IsUnique();

        builder.HasIndex(c => c.PhoneNumber)
            .HasDatabaseName("IX_Customers_PhoneNumber");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Customers_Status");

        builder.HasIndex(c => c.RegisteredAtUtc)
            .HasDatabaseName("IX_Customers_RegisteredAtUtc");

        builder.HasIndex(c => c.DateOfBirth)
            .HasDatabaseName("IX_Customers_DateOfBirth");

        // Note: Indexes on complex property members (Address.City, Address.PostalCode, DriversLicense.ExpiryDate)
        // need to be added via migration Up() method using SQL if needed for performance

        // Ignore domain events (not persisted)
        builder.Ignore(c => c.DomainEvents);

        // Computed properties (not persisted)
        builder.Ignore(c => c.FullName);
        builder.Ignore(c => c.Age);
    }
}
