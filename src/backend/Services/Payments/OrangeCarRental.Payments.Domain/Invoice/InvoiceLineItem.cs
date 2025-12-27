using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     A line item on an invoice.
///     German invoices require detailed breakdown of services.
/// </summary>
public sealed record InvoiceLineItem : IValueObject
{
    /// <summary>
    ///     Line item number (1-based).
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    ///     Description of the service/item.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    ///     Quantity with unit of measure.
    /// </summary>
    public Quantity Quantity { get; init; }

    /// <summary>
    ///     Net price per unit.
    /// </summary>
    public Money UnitPrice { get; init; }

    /// <summary>
    ///     VAT rate for this line item.
    /// </summary>
    public VatRate VatRate { get; init; }

    /// <summary>
    ///     Total net amount for this line (Quantity * UnitPriceNet).
    /// </summary>
    public decimal TotalNet => Quantity.Value * UnitPrice.NetAmount;

    /// <summary>
    ///     VAT amount for this line.
    /// </summary>
    public decimal VatAmount => VatRate.CalculateVatAmount(TotalNet);

    /// <summary>
    ///     Total gross amount for this line (Net + VAT).
    /// </summary>
    public decimal TotalGross => TotalNet + VatAmount;

    /// <summary>
    ///     Service period start date (for car rental).
    /// </summary>
    public DateOnly? ServicePeriodStart { get; init; }

    /// <summary>
    ///     Service period end date (for car rental).
    /// </summary>
    public DateOnly? ServicePeriodEnd { get; init; }

    /// <summary>
    ///     Creates a line item for vehicle rental.
    /// </summary>
    public static InvoiceLineItem ForVehicleRental(
        int position,
        string vehicleDescription,
        int rentalDays,
        decimal dailyRateNet,
        DateOnly pickupDate,
        DateOnly returnDate)
    {
        return new InvoiceLineItem
        {
            Position = position,
            Description = $"Fahrzeugmiete: {vehicleDescription}",
            Quantity = Quantity.Days(rentalDays),
            UnitPrice = Money.Euro(dailyRateNet),
            VatRate = VatRate.GermanStandard,
            ServicePeriodStart = pickupDate,
            ServicePeriodEnd = returnDate
        };
    }

    /// <summary>
    ///     Creates a line item for additional services.
    /// </summary>
    public static InvoiceLineItem ForAdditionalService(
        int position,
        string description,
        Quantity quantity,
        decimal unitPriceNet)
    {
        return new InvoiceLineItem
        {
            Position = position,
            Description = description,
            Quantity = quantity,
            UnitPrice = Money.Euro(unitPriceNet),
            VatRate = VatRate.GermanStandard
        };
    }

    /// <summary>
    ///     Creates a line item for insurance.
    /// </summary>
    public static InvoiceLineItem ForInsurance(
        int position,
        string insuranceDescription,
        int rentalDays,
        decimal dailyRateNet)
    {
        return new InvoiceLineItem
        {
            Position = position,
            Description = $"Versicherung: {insuranceDescription}",
            Quantity = Quantity.Days(rentalDays),
            UnitPrice = Money.Euro(dailyRateNet),
            VatRate = VatRate.GermanStandard
        };
    }

    /// <summary>
    ///     Creates a line item for kilometer surcharge.
    /// </summary>
    public static InvoiceLineItem ForKilometerSurcharge(
        int position,
        int extraKilometers,
        decimal pricePerKilometer)
    {
        return new InvoiceLineItem
        {
            Position = position,
            Description = "Kilometerüberschreitung",
            Quantity = Quantity.Kilometers(extraKilometers),
            UnitPrice = Money.Euro(pricePerKilometer),
            VatRate = VatRate.GermanStandard
        };
    }
}
