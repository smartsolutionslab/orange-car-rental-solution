using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

/// <summary>
///     International Bank Account Number (IBAN) value object.
///     Validates format and check digits according to ISO 13616.
/// </summary>
public sealed partial record IBAN : IValueObject
{
    private IBAN(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    ///     Gets the country code (first 2 characters).
    /// </summary>
    public string CountryCode => Value[..2];

    /// <summary>
    ///     Gets the check digits (characters 3-4).
    /// </summary>
    public string CheckDigits => Value[2..4];

    /// <summary>
    ///     Gets the Basic Bank Account Number (remaining characters).
    /// </summary>
    public string BBAN => Value[4..];

    /// <summary>
    ///     Checks if this is a German IBAN.
    /// </summary>
    public bool IsGerman => CountryCode == "DE";

    /// <summary>
    ///     Gets the formatted IBAN with spaces every 4 characters.
    /// </summary>
    public string Formatted => FormatIBAN(Value);

    /// <summary>
    ///     Creates an IBAN from the given value.
    /// </summary>
    /// <param name="value">The IBAN string (with or without spaces).</param>
    /// <returns>A validated IBAN.</returns>
    /// <exception cref="ArgumentException">If the IBAN is invalid.</exception>
    public static IBAN Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        // Remove spaces and convert to uppercase
        var normalized = value.Replace(" ", "").ToUpperInvariant();

        // Basic format validation
        if (!IBANFormatRegex().IsMatch(normalized))
            throw new ArgumentException("Invalid IBAN format. Must be 2 letters followed by 2 digits and up to 30 alphanumeric characters.", nameof(value));

        // German IBAN specific validation (DE + 20 characters = 22 total)
        if (normalized.StartsWith("DE") && normalized.Length != 22)
            throw new ArgumentException("German IBAN must be exactly 22 characters.", nameof(value));

        // Validate check digits using MOD-97 algorithm
        if (!ValidateCheckDigits(normalized))
            throw new ArgumentException("Invalid IBAN check digits.", nameof(value));

        return new IBAN(normalized);
    }

    /// <summary>
    ///     Tries to create an IBAN from the given value.
    /// </summary>
    public static bool TryCreate(string? value, out IBAN? iban)
    {
        iban = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            iban = Create(value);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Parses an IBAN from the given string.
    /// </summary>
    public static IBAN Parse(string value) => Create(value);

    /// <summary>
    ///     Validates the IBAN check digits using MOD-97 algorithm (ISO 7064).
    /// </summary>
    private static bool ValidateCheckDigits(string iban)
    {
        // Move first 4 characters to end
        var rearranged = iban[4..] + iban[..4];

        // Convert letters to numbers (A=10, B=11, ..., Z=35)
        var numericString = string.Concat(rearranged.Select(c =>
            char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        // Calculate MOD 97 on the numeric string
        // Use BigInteger-like approach for large numbers
        var remainder = 0;
        foreach (var c in numericString)
        {
            remainder = (remainder * 10 + (c - '0')) % 97;
        }

        return remainder == 1;
    }

    private static string FormatIBAN(string iban)
    {
        var formatted = new System.Text.StringBuilder();
        for (var i = 0; i < iban.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
                formatted.Append(' ');
            formatted.Append(iban[i]);
        }
        return formatted.ToString();
    }

    public override string ToString() => Formatted;

    [GeneratedRegex(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$")]
    private static partial Regex IBANFormatRegex();
}
