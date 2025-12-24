using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     A line item on an invoice.
///     German invoices require detailed breakdown of services.
/// </summary>
public sealed record InvoiceLineItem
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
    ///     Quantity (e.g., number of rental days).
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    ///     Unit of measure (e.g., "Tag" for day, "Stück" for piece).
    /// </summary>
    public string Unit { get; init; } = "Tag";

    /// <summary>
    ///     Net price per unit.
    /// </summary>
    public decimal UnitPriceNet { get; init; }

    /// <summary>
    ///     VAT rate (e.g., 0.19 for 19%).
    /// </summary>
    public decimal VatRate { get; init; } = 0.19m;

    /// <summary>
    ///     Total net amount for this line (Quantity * UnitPriceNet).
    /// </summary>
    public decimal TotalNet => Quantity * UnitPriceNet;

    /// <summary>
    ///     VAT amount for this line.
    /// </summary>
    public decimal VatAmount => Math.Round(TotalNet * VatRate, 2);

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
            Quantity = rentalDays,
            Unit = rentalDays == 1 ? "Tag" : "Tage",
            UnitPriceNet = dailyRateNet,
            VatRate = 0.19m,
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
        int quantity,
        string unit,
        decimal unitPriceNet)
    {
        return new InvoiceLineItem
        {
            Position = position,
            Description = description,
            Quantity = quantity,
            Unit = unit,
            UnitPriceNet = unitPriceNet,
            VatRate = 0.19m
        };
    }
}
