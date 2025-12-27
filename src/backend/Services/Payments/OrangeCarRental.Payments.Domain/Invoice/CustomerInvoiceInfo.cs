using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Customer information for invoices.
///     Contains billing address and optional VAT ID for business customers.
/// </summary>
public sealed record CustomerInvoiceInfo : IValueObject
{
    /// <summary>
    ///     Customer identifier (reference to Customers bounded context).
    /// </summary>
    public CustomerId CustomerId { get; }

    /// <summary>
    ///     Customer name (person or company name).
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Street address.
    /// </summary>
    public string Street { get; }

    /// <summary>
    ///     Postal code.
    /// </summary>
    public string PostalCode { get; }

    /// <summary>
    ///     City.
    /// </summary>
    public string City { get; }

    /// <summary>
    ///     Country.
    /// </summary>
    public string Country { get; }

    /// <summary>
    ///     VAT ID for business customers (optional, e.g., "DE123456789").
    /// </summary>
    public string? VatId { get; }

    private CustomerInvoiceInfo(
        CustomerId customerId,
        string name,
        string street,
        string postalCode,
        string city,
        string country,
        string? vatId)
    {
        CustomerId = customerId;
        Name = name;
        Street = street;
        PostalCode = postalCode;
        City = city;
        Country = country;
        VatId = vatId;
    }

    /// <summary>
    ///     Creates customer invoice information.
    /// </summary>
    public static CustomerInvoiceInfo Create(
        CustomerId customerId,
        string name,
        string street,
        string postalCode,
        string city,
        string country,
        string? vatId = null)
    {
        Ensure.That(customerId.Value, nameof(customerId))
            .ThrowIf(customerId.Value == Guid.Empty, "Customer ID cannot be empty");
        Ensure.That(name, nameof(name)).IsNotNullOrWhiteSpace();
        Ensure.That(street, nameof(street)).IsNotNullOrWhiteSpace();
        Ensure.That(postalCode, nameof(postalCode)).IsNotNullOrWhiteSpace();
        Ensure.That(city, nameof(city)).IsNotNullOrWhiteSpace();
        Ensure.That(country, nameof(country)).IsNotNullOrWhiteSpace();

        return new CustomerInvoiceInfo(
            customerId,
            name.Trim(),
            street.Trim(),
            postalCode.Trim(),
            city.Trim(),
            country.Trim(),
            vatId?.Trim());
    }

    /// <summary>
    ///     Gets whether this is a business customer (has VAT ID).
    /// </summary>
    public bool IsBusinessCustomer => !string.IsNullOrWhiteSpace(VatId);

    /// <summary>
    ///     Gets the formatted address.
    /// </summary>
    public string FormattedAddress => $"{Street}, {PostalCode} {City}, {Country}";
}
