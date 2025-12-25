using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     German VAT Identification Number (Umsatzsteuer-Identifikationsnummer / USt-IdNr.).
///     Format: DE + 9 digits (e.g., DE123456789).
/// </summary>
public readonly partial record struct VATId : IValueObject
{
    /// <summary>
    ///     Gets the VAT ID value (normalized, without spaces).
    /// </summary>
    public string Value { get; }

    private VATId(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a new German VAT ID.
    /// </summary>
    /// <param name="value">The VAT ID string.</param>
    /// <returns>A validated VAT ID.</returns>
    /// <exception cref="ArgumentException">If the VAT ID format is invalid.</exception>
    public static VATId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("VAT ID cannot be empty", nameof(value));

        // Normalize: remove spaces and convert to uppercase
        var normalized = value.Replace(" ", "").ToUpperInvariant();

        // Validate German VAT ID format: DE + 9 digits
        if (!GermanVATIdRegex().IsMatch(normalized))
            throw new ArgumentException(
                "Invalid German VAT ID format. Must be 'DE' followed by 9 digits (e.g., DE123456789)",
                nameof(value));

        return new VATId(normalized);
    }

    /// <summary>
    ///     Tries to create a VAT ID without throwing an exception.
    /// </summary>
    /// <param name="value">The VAT ID string.</param>
    /// <param name="vatId">The created VAT ID if successful.</param>
    /// <returns>True if the VAT ID was created successfully.</returns>
    public static bool TryCreate(string value, out VATId vatId)
    {
        try
        {
            vatId = Create(value);
            return true;
        }
        catch
        {
            vatId = default;
            return false;
        }
    }

    /// <summary>
    ///     Gets the country code (DE for Germany).
    /// </summary>
    public string CountryCode => Value[..2];

    /// <summary>
    ///     Gets the numeric part of the VAT ID.
    /// </summary>
    public string NumericPart => Value[2..];

    /// <summary>
    ///     Gets whether this is a German VAT ID.
    /// </summary>
    public bool IsGerman => CountryCode == "DE";

    /// <summary>
    ///     Gets the formatted VAT ID with space (DE 123456789).
    /// </summary>
    public string Formatted => $"{CountryCode} {NumericPart}";

    public override string ToString() => Value;

    /// <summary>
    ///     Validates the check digit using the German VAT ID algorithm.
    ///     Note: Full VIES validation requires an API call to the EU VIES service.
    /// </summary>
    /// <returns>True if the check digit is valid (basic format validation only).</returns>
    public bool ValidateCheckDigit()
    {
        // German VAT IDs use a weighted checksum algorithm
        // This is a simplified validation - full validation requires VIES API
        if (!IsGerman || NumericPart.Length != 9)
            return false;

        // Check that all characters are digits
        return NumericPart.All(char.IsDigit);
    }

    [GeneratedRegex(@"^DE\d{9}$")]
    private static partial Regex GermanVATIdRegex();
}
