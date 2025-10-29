namespace OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Represents a currency using ISO 4217 codes.
/// </summary>
public sealed record Currency
{
    /// <summary>
    /// Three-letter currency code (ISO 4217).
    /// </summary>
    public string Code { get; init; }

    public Currency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Currency code cannot be empty", nameof(code));

        if (code.Length != 3)
            throw new ArgumentException("Currency code must be exactly 3 characters (ISO 4217)", nameof(code));

        Code = code.ToUpperInvariant();
    }

    /// <summary>
    /// Euro currency.
    /// </summary>
    public static readonly Currency EUR = new("EUR");

    /// <summary>
    /// US Dollar currency.
    /// </summary>
    public static readonly Currency USD = new("USD");

    /// <summary>
    /// British Pound currency.
    /// </summary>
    public static readonly Currency GBP = new("GBP");

    /// <summary>
    /// Swiss Franc currency.
    /// </summary>
    public static readonly Currency CHF = new("CHF");

    public override string ToString() => Code;
}
