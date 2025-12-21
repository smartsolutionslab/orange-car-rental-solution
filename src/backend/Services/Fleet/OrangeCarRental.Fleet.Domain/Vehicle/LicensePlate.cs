using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     License plate value object.
///     Validates and normalizes vehicle license plate numbers.
///     Supports German license plate format (e.g., "B AB 1234", "M XY 999").
/// </summary>
/// <param name="Value">The normalized license plate value (uppercase, trimmed).</param>
public readonly partial record struct LicensePlate(string Value) : IValueObject
{
    private const int MaxLength = 20;

    [GeneratedRegex(@"^[A-ZÄÖÜ]{1,3}[\s-]?[A-ZÄÖÜ]{1,2}[\s-]?\d{1,4}[EH]?$", RegexOptions.IgnoreCase)]
    private static partial Regex continue
    GermanLicensePlatePattern();

    public static LicensePlate From(string value)
    {
        var trimmed = value?.Trim().ToUpperInvariant() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(MaxLength);

        if (!GermanLicensePlatePattern().IsMatch(trimmed))
        {
            throw new ArgumentException(
                $"Invalid license plate format: '{value}'. Expected German format (e.g., 'B AB 1234').",
                nameof(value));
        }

        return new LicensePlate(trimmed);
    }

    public static LicensePlate? FromNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return From(value);
    }

    public static bool TryParse(string? value, out LicensePlate result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim().ToUpperInvariant();

        if (trimmed.Length > MaxLength)
            return false;

        if (!GermanLicensePlatePattern().IsMatch(trimmed))
            return false;

        result = new LicensePlate(trimmed);
        return true;
    }

    public static implicit operator string(LicensePlate licensePlate) => licensePlate.Value;

    public override string ToString() => Value;
}
