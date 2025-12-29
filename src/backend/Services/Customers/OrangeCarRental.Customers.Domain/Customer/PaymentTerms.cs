using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Payment terms for business customers (Zahlungsziel).
///     Defines the number of days until payment is due.
/// </summary>
public readonly record struct PaymentTerms : IValueObject
{
    /// <summary>
    ///     Gets the number of days until payment is due.
    /// </summary>
    public int DaysUntilDue { get; }

    private PaymentTerms(int daysUntilDue)
    {
        DaysUntilDue = daysUntilDue;
    }

    /// <summary>
    ///     Creates custom payment terms.
    /// </summary>
    /// <param name="daysUntilDue">Number of days until payment is due.</param>
    /// <returns>Payment terms.</returns>
    /// <exception cref="ArgumentException">If days is negative or exceeds 90.</exception>
    public static PaymentTerms Create(int daysUntilDue)
    {
        Ensure.That(daysUntilDue, nameof(daysUntilDue))
            .IsNotNegative()
            .AndIsLessThanOrEqual(90);

        return new PaymentTerms(daysUntilDue);
    }

    /// <summary>
    ///     Payment due immediately upon receipt of invoice.
    /// </summary>
    public static readonly PaymentTerms Immediate = new(0);

    /// <summary>
    ///     Payment due within 7 days (Zahlungsziel 7 Tage).
    /// </summary>
    public static readonly PaymentTerms Net7 = new(7);

    /// <summary>
    ///     Payment due within 14 days (Zahlungsziel 14 Tage).
    /// </summary>
    public static readonly PaymentTerms Net14 = new(14);

    /// <summary>
    ///     Payment due within 30 days (Zahlungsziel 30 Tage).
    /// </summary>
    public static readonly PaymentTerms Net30 = new(30);

    /// <summary>
    ///     Payment due within 60 days (Zahlungsziel 60 Tage).
    /// </summary>
    public static readonly PaymentTerms Net60 = new(60);

    /// <summary>
    ///     Gets whether this is immediate payment.
    /// </summary>
    public bool IsImmediate => DaysUntilDue == 0;

    /// <summary>
    ///     Calculates the due date from an invoice date.
    /// </summary>
    /// <param name="invoiceDate">The invoice date.</param>
    /// <returns>The payment due date.</returns>
    public DateOnly CalculateDueDate(DateOnly invoiceDate)
    {
        return invoiceDate.AddDays(DaysUntilDue);
    }

    /// <summary>
    ///     Gets the German display name.
    /// </summary>
    public string GetGermanDisplayName() => DaysUntilDue switch
    {
        0 => "Sofort fällig",
        7 => "Zahlungsziel 7 Tage",
        14 => "Zahlungsziel 14 Tage",
        30 => "Zahlungsziel 30 Tage",
        60 => "Zahlungsziel 60 Tage",
        _ => $"Zahlungsziel {DaysUntilDue} Tage"
    };

    /// <summary>
    ///     Gets the English display name.
    /// </summary>
    public string GetEnglishDisplayName() => DaysUntilDue switch
    {
        0 => "Due immediately",
        _ => $"Net {DaysUntilDue} days"
    };

    public override string ToString() => GetEnglishDisplayName();
}
