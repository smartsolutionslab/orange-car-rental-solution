using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

/// <summary>
///     Bank Identifier Code (BIC/SWIFT) value object.
///     Validates format according to ISO 9362.
/// </summary>
public sealed partial record BIC : IValueObject
{
    private BIC(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    ///     Gets the bank code (first 4 characters).
    /// </summary>
    public string BankCode => Value[..4];

    /// <summary>
    ///     Gets the country code (characters 5-6).
    /// </summary>
    public string CountryCode => Value[4..6];

    /// <summary>
    ///     Gets the location code (characters 7-8).
    /// </summary>
    public string LocationCode => Value[6..8];

    /// <summary>
    ///     Gets the branch code (characters 9-11, or "XXX" if not present).
    /// </summary>
    public string BranchCode => Value.Length == 11 ? Value[8..11] : "XXX";

    /// <summary>
    ///     Checks if this is a German BIC.
    /// </summary>
    public bool IsGerman => CountryCode == "DE";

    /// <summary>
    ///     Creates a BIC from the given value.
    /// </summary>
    /// <param name="value">The BIC string.</param>
    /// <returns>A validated BIC.</returns>
    /// <exception cref="ArgumentException">If the BIC is invalid.</exception>
    public static BIC Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        // Remove spaces and convert to uppercase
        var normalized = value.Replace(" ", "").ToUpperInvariant();

        // BIC must be 8 or 11 characters
        if (normalized.Length != 8 && normalized.Length != 11)
            throw new ArgumentException("BIC must be 8 or 11 characters.", nameof(value));

        // Validate format: 4 letters (bank) + 2 letters (country) + 2 alphanumeric (location) + optional 3 alphanumeric (branch)
        if (!BICFormatRegex().IsMatch(normalized))
            throw new ArgumentException("Invalid BIC format.", nameof(value));

        return new BIC(normalized);
    }

    /// <summary>
    ///     Tries to create a BIC from the given value.
    /// </summary>
    public static bool TryCreate(string? value, out BIC? bic)
    {
        bic = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            bic = Create(value);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Parses a BIC from the given string.
    /// </summary>
    public static BIC Parse(string value) => Create(value);

    public override string ToString() => Value;

    [GeneratedRegex(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$")]
    private static partial Regex BICFormatRegex();
}
