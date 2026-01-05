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
    public CustomerIdentifier CustomerIdentifier { get; }

    /// <summary>
    ///     Customer name (person or company name).
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Street address.
    /// </summary>
    public Street Street { get; }

    /// <summary>
    ///     Postal code.
    /// </summary>
    public PostalCode PostalCode { get; }

    /// <summary>
    ///     City.
    /// </summary>
    public City City { get; }

    /// <summary>
    ///     Country.
    /// </summary>
    public Country Country { get; }

    /// <summary>
    ///     VAT ID for business customers (optional, e.g., "DE123456789").
    /// </summary>
    public VatId? VatId { get; }

    private CustomerInvoiceInfo(
        CustomerIdentifier customerId,
        string name,
        Street street,
        PostalCode postalCode,
        City city,
        Country country,
        VatId? vatId)
    {
        CustomerIdentifier = customerId;
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
        CustomerIdentifier customerId,
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

        return new CustomerInvoiceInfo(
            customerId,
            name.Trim(),
            Street.From(street),
            PostalCode.From(postalCode),
            City.From(city),
            Country.From(country),
            Common.VatId.FromNullable(vatId));
    }

    /// <summary>
    ///     Gets whether this is a business customer (has VAT ID).
    /// </summary>
    public bool IsBusinessCustomer => VatId.HasValue;

    /// <summary>
    ///     Gets the formatted address.
    /// </summary>
    public string FormattedAddress => $"{Street}, {PostalCode} {City}, {Country}";
}
