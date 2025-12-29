using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     VAT Identification Number (Umsatzsteuer-Identifikationsnummer / USt-IdNr.) for invoices.
///     German format: DE + 9 digits (e.g., DE123456789).
/// </summary>
public readonly partial record struct VatId : IValueObject
{
    public string Value { get; }

    private VatId(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a VAT ID from a string value.
    /// </summary>
    public static VatId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("VAT ID cannot be empty", nameof(value));

        var normalized = value.Replace(" ", "").ToUpperInvariant();

        if (!GermanVatIdRegex().IsMatch(normalized))
            throw new ArgumentException(
                "Invalid German VAT ID format. Must be 'DE' followed by 9 digits (e.g., DE123456789)",
                nameof(value));

        return new VatId(normalized);
    }

    /// <summary>
    ///     Creates a VAT ID from a nullable string, returning null if empty.
    /// </summary>
    public static VatId? FromNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return From(value);
    }

    /// <summary>
    ///     Gets the formatted VAT ID with space (DE 123456789).
    /// </summary>
    public string Formatted => $"{Value[..2]} {Value[2..]}";

    public static implicit operator string(VatId vatId) => vatId.Value;

    public override string ToString() => Value;

    [GeneratedRegex(@"^DE\d{9}$")]
    private static partial Regex GermanVatIdRegex();
}
