using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Seller/company information for invoices.
///     Contains all legally required business information for German invoices (Impressum).
/// </summary>
public sealed record SellerInfo : IValueObject
{
    /// <summary>
    ///     Company name (e.g., "Orange Car Rental GmbH").
    /// </summary>
    public string CompanyName { get; }

    /// <summary>
    ///     Street address (e.g., "Musterstraße 123").
    /// </summary>
    public string Street { get; }

    /// <summary>
    ///     Postal code (e.g., "10115").
    /// </summary>
    public string PostalCode { get; }

    /// <summary>
    ///     City (e.g., "Berlin").
    /// </summary>
    public string City { get; }

    /// <summary>
    ///     Country (e.g., "Deutschland").
    /// </summary>
    public string Country { get; }

    /// <summary>
    ///     Email address for invoices.
    /// </summary>
    public string Email { get; }

    /// <summary>
    ///     Phone number.
    /// </summary>
    public string Phone { get; }

    /// <summary>
    ///     Trade register number (Handelsregisternummer, e.g., "HRB 123456 B").
    /// </summary>
    public string TradeRegisterNumber { get; }

    /// <summary>
    ///     VAT ID (USt-IdNr., e.g., "DE123456789").
    /// </summary>
    public string VatId { get; }

    /// <summary>
    ///     Tax number (Steuernummer, e.g., "27/123/45678").
    /// </summary>
    public string TaxNumber { get; }

    /// <summary>
    ///     Managing director name (Geschäftsführer).
    /// </summary>
    public PersonName ManagingDirector { get; }

    private SellerInfo(
        string companyName,
        string street,
        string postalCode,
        string city,
        string country,
        string email,
        string phone,
        string tradeRegisterNumber,
        string vatId,
        string taxNumber,
        PersonName managingDirector)
    {
        CompanyName = companyName;
        Street = street;
        PostalCode = postalCode;
        City = city;
        Country = country;
        Email = email;
        Phone = phone;
        TradeRegisterNumber = tradeRegisterNumber;
        VatId = vatId;
        TaxNumber = taxNumber;
        ManagingDirector = managingDirector;
    }

    /// <summary>
    ///     Creates seller information with all required fields.
    /// </summary>
    public static SellerInfo Create(
        string companyName,
        string street,
        string postalCode,
        string city,
        string country,
        string email,
        string phone,
        string tradeRegisterNumber,
        string vatId,
        string taxNumber,
        string managingDirector)
    {
        Ensure.That(companyName, nameof(companyName)).IsNotNullOrWhiteSpace();
        Ensure.That(street, nameof(street)).IsNotNullOrWhiteSpace();
        Ensure.That(postalCode, nameof(postalCode)).IsNotNullOrWhiteSpace();
        Ensure.That(city, nameof(city)).IsNotNullOrWhiteSpace();
        Ensure.That(country, nameof(country)).IsNotNullOrWhiteSpace();
        Ensure.That(email, nameof(email)).IsNotNullOrWhiteSpace();
        Ensure.That(phone, nameof(phone)).IsNotNullOrWhiteSpace();
        Ensure.That(tradeRegisterNumber, nameof(tradeRegisterNumber)).IsNotNullOrWhiteSpace();
        Ensure.That(vatId, nameof(vatId)).IsNotNullOrWhiteSpace();
        Ensure.That(taxNumber, nameof(taxNumber)).IsNotNullOrWhiteSpace();

        return new SellerInfo(
            companyName.Trim(),
            street.Trim(),
            postalCode.Trim(),
            city.Trim(),
            country.Trim(),
            email.Trim(),
            phone.Trim(),
            tradeRegisterNumber.Trim(),
            vatId.Trim(),
            taxNumber.Trim(),
            PersonName.Of(managingDirector));
    }

    /// <summary>
    ///     Default Orange Car Rental seller information.
    /// </summary>
    public static SellerInfo OrangeCarRentalDefault => Create(
        companyName: "Orange Car Rental GmbH",
        street: "Musterstraße 123",
        postalCode: "10115",
        city: "Berlin",
        country: "Deutschland",
        email: "rechnung@orangecarrental.de",
        phone: "+49 30 12345678",
        tradeRegisterNumber: "HRB 123456 B",
        vatId: "DE123456789",
        taxNumber: "27/123/45678",
        managingDirector: "Max Mustermann");

    /// <summary>
    ///     Gets the formatted address.
    /// </summary>
    public string FormattedAddress => $"{Street}, {PostalCode} {City}, {Country}";
}
