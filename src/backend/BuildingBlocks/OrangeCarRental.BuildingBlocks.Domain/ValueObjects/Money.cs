namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Represents monetary value with VAT support for German market compliance.
/// Always stores amounts in the smallest currency unit (e.g., cents for EUR).
/// </summary>
/// <param name="NetAmount">Net amount (before VAT).</param>
/// <param name="VatAmount">VAT amount.</param>
/// <param name="Currency">Currency code (ISO 4217).</param>
public readonly record struct Money(decimal NetAmount, decimal VatAmount, Currency Currency)
{
    /// <summary>
    /// Gross amount (net + VAT).
    /// </summary>
    public decimal GrossAmount => NetAmount + VatAmount;

    /// <summary>
    /// VAT rate (calculated from net and VAT amounts).
    /// </summary>
    public decimal VatRate => NetAmount > 0 ? VatAmount / NetAmount : 0;

    /// <summary>
    /// Creates money with VAT applied from net amount.
    /// </summary>
    /// <param name="netAmount">Net amount before VAT.</param>
    /// <param name="vatRate">VAT rate (e.g., 0.19 for 19%).</param>
    /// <param name="currency">Currency.</param>
    public static Money Of(decimal netAmount, decimal vatRate, Currency currency)
    {
        if (netAmount < 0)
            throw new ArgumentException("Net amount cannot be negative", nameof(netAmount));

        var roundedNetAmount = Math.Round(netAmount, 2);
        var vatAmount = Math.Round(roundedNetAmount * vatRate, 2);

        return new Money(roundedNetAmount, vatAmount, currency);
    }

    /// <summary>
    /// Creates money from gross amount (includes VAT).
    /// </summary>
    /// <param name="grossAmount">Total amount including VAT.</param>
    /// <param name="vatRate">VAT rate (e.g., 0.19 for 19%).</param>
    /// <param name="currency">Currency.</param>
    public static Money FromGross(decimal grossAmount, decimal vatRate, Currency currency)
    {
        if (grossAmount < 0)
            throw new ArgumentException("Gross amount cannot be negative", nameof(grossAmount));

        var netAmount = Math.Round(grossAmount / (1 + vatRate), 2);
        var vatAmount = Math.Round(grossAmount - netAmount, 2);

        return new Money(netAmount, vatAmount, currency);
    }

    /// <summary>
    /// Creates money in EUR with German VAT (19%) from net amount.
    /// </summary>
    /// <param name="netAmount">Net amount in EUR.</param>
    public static Money Euro(decimal netAmount)
    {
        return Of(netAmount, 0.19m, Currency.EUR);
    }

    /// <summary>
    /// Creates money in EUR from gross amount with German VAT (19%).
    /// </summary>
    /// <param name="grossAmount">Gross amount in EUR.</param>
    public static Money EuroGross(decimal grossAmount)
    {
        return FromGross(grossAmount, 0.19m, Currency.EUR);
    }

    /// <summary>
    /// Creates zero money.
    /// </summary>
    public static Money Zero(Currency currency)
    {
        return new Money(0, 0, currency);
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency.Code != b.Currency.Code)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(a.NetAmount + b.NetAmount, a.VatAmount + b.VatAmount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency.Code != b.Currency.Code)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(a.NetAmount - b.NetAmount, a.VatAmount - b.VatAmount, a.Currency);
    }

    public static Money operator *(Money money, int multiplier)
    {
        return new Money(money.NetAmount * multiplier, money.VatAmount * multiplier, money.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(
            Math.Round(money.NetAmount * multiplier, 2),
            Math.Round(money.VatAmount * multiplier, 2),
            money.Currency);
    }

    /// <summary>
    /// Formats the money for German market (e.g., "119,00 €").
    /// </summary>
    public string ToGermanString()
    {
        return $"{GrossAmount:N2} {Currency.Code}".Replace(",", ".");
    }

    public override string ToString()
    {
        return $"{GrossAmount:F2} {Currency.Code}";
    }
}
