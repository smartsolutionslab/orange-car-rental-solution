namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Configuration;

/// <summary>
///     Configuration options for SEPA Direct Debit processing.
/// </summary>
public sealed class SepaConfiguration
{
    /// <summary>
    ///     Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Sepa";

    /// <summary>
    ///     Creditor Identifier (Gläubiger-Identifikationsnummer).
    ///     Unique identifier assigned by the Bundesbank.
    ///     Format: DE + 2 check digits + 3 chars creditor business code + 11 chars national identifier.
    /// </summary>
    public string CreditorId { get; set; } = "DE98ZZZ09999999999";

    /// <summary>
    ///     Creditor company name used in SEPA mandate documents.
    /// </summary>
    public string CreditorName { get; set; } = "Orange Car Rental GmbH";

    /// <summary>
    ///     Number of months after which an unused SEPA mandate expires.
    ///     Per SEPA Core Direct Debit rulebook, mandates expire after 36 months of inactivity.
    /// </summary>
    public int MandateExpiryMonths { get; set; } = 36;
}
