using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

/// <summary>
///     SEPA Mandate Reference (Mandatsreferenz) value object.
///     Must be unique per creditor and can contain alphanumeric characters and certain special characters.
///     Format: OCR-SEPA-YYYYMMDD-XXXXXX (company prefix + date + sequence)
/// </summary>
public sealed partial record MandateReference : IValueObject
{
    private const string Prefix = "OCR-SEPA";

    private MandateReference(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    ///     Creates a new mandate reference with a sequence number.
    /// </summary>
    /// <param name="sequenceNumber">Unique sequence number for this mandate.</param>
    /// <param name="date">The date to include in the reference (defaults to today).</param>
    /// <returns>A new mandate reference.</returns>
    public static MandateReference Create(int sequenceNumber, DateOnly? date = null)
    {
        Ensure.That(sequenceNumber, nameof(sequenceNumber)).IsGreaterThan(0);

        var mandateDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var value = $"{Prefix}-{mandateDate:yyyyMMdd}-{sequenceNumber:D6}";

        return new MandateReference(value);
    }

    /// <summary>
    ///     Parses a mandate reference from a string.
    /// </summary>
    /// <param name="value">The mandate reference string.</param>
    /// <returns>A validated mandate reference.</returns>
    /// <exception cref="ArgumentException">If the format is invalid.</exception>
    public static MandateReference Parse(string value)
    {
        Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

        var normalized = value.Trim().ToUpperInvariant();

        Ensure.That(normalized, nameof(value))
            .AndSatisfies(v => MandateReferenceRegex().IsMatch(v), $"Invalid mandate reference format. Expected: {Prefix}-YYYYMMDD-NNNNNN");

        return new MandateReference(normalized);
    }

    /// <summary>
    ///     Tries to parse a mandate reference from a string.
    /// </summary>
    public static bool TryParse(string? value, out MandateReference? reference)
    {
        reference = null;
        if (string.IsNullOrWhiteSpace(value)) return false;

        try
        {
            reference = Parse(value);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^OCR-SEPA-\d{8}-\d{6}$")]
    private static partial Regex MandateReferenceRegex();
}
