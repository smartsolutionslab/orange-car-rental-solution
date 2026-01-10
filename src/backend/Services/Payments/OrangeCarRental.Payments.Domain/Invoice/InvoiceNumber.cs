using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Consecutive invoice number as required by German tax law.
///     Format: OCR-YYYY-NNNNNN (e.g., OCR-2025-000001)
/// </summary>
public sealed record InvoiceNumber : IValueObject
{
    private const string Prefix = "OCR";

    public string Value { get; }
    public int Year { get; }
    public int SequenceNumber { get; }

    private InvoiceNumber(string value, int year, int sequenceNumber)
    {
        Value = value;
        Year = year;
        SequenceNumber = sequenceNumber;
    }

    /// <summary>
    ///     Creates a new invoice number with the given sequence number for the current year.
    /// </summary>
    public static InvoiceNumber Create(int sequenceNumber, int? year = null)
    {
        var invoiceYear = year ?? DateTime.UtcNow.Year;

        Ensure.That(sequenceNumber, nameof(sequenceNumber)).IsPositive();
        Ensure.That(invoiceYear, nameof(year))
            .IsGreaterThanOrEqual(2020)
            .AndIsLessThanOrEqual(2099);

        var value = $"{Prefix}-{invoiceYear}-{sequenceNumber:D6}";
        return new InvoiceNumber(value, invoiceYear, sequenceNumber);
    }

    /// <summary>
    ///     Parses an invoice number from its string representation.
    /// </summary>
    public static InvoiceNumber Parse(string value)
    {
        Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

        var parts = value.Split('-');

        Ensure.That(parts, nameof(value))
            .AndSatisfies(p => p.Length == 3 && p[0] == Prefix, $"Invalid invoice number format: {value}");

        Ensure.That(parts[1], nameof(value))
            .AndSatisfies(p => int.TryParse(p, out _), $"Invalid year in invoice number: {value}");

        Ensure.That(parts[2], nameof(value))
            .AndSatisfies(p => int.TryParse(p, out _), $"Invalid sequence number in invoice number: {value}");

        var year = int.Parse(parts[1]);
        var sequenceNumber = int.Parse(parts[2]);

        return new InvoiceNumber(value, year, sequenceNumber);
    }

    public override string ToString() => Value;
}
