using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Driver's license value object.
///     Represents a driver's license with validation for EU/German requirements.
/// </summary>
/// <param name="LicenseNumber">The license number.</param>
/// <param name="IssueCountry">The country that issued the license.</param>
/// <param name="IssueDate">The date the license was issued.</param>
/// <param name="ExpiryDate">The date the license expires.</param>
public readonly record struct DriversLicense(
    string LicenseNumber,
    string IssueCountry,
    DateOnly IssueDate,
    DateOnly ExpiryDate) : IValueObject
{
    /// <summary>
    ///     Creates a driver's license value object.
    /// </summary>
    /// <param name="licenseNumber">The license number (alphanumeric).</param>
    /// <param name="issueCountry">The country that issued the license.</param>
    /// <param name="issueDate">The date the license was issued.</param>
    /// <param name="expiryDate">The date the license expires.</param>
    /// <exception cref="ArgumentException">Thrown when any field is invalid.</exception>
    public static DriversLicense Of(
        string licenseNumber,
        string issueCountry,
        DateOnly issueDate,
        DateOnly expiryDate)
    {
        var normalizedLicenseNumber = licenseNumber?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedIssueCountry = issueCountry?.Trim() ?? string.Empty;

        Ensure.That(normalizedLicenseNumber, nameof(licenseNumber))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(5, 20)
            .AndSatisfies(
                num => num.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)),
                "License number must contain only letters, digits, and spaces");

        Ensure.That(normalizedIssueCountry, nameof(issueCountry))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minimumIssueDate = new DateOnly(1950, 1, 1);

        Ensure.That(issueDate, nameof(issueDate))
            .IsGreaterThanOrEqual(minimumIssueDate)
            .IsLessThanOrEqual(today);

        Ensure.That(expiryDate, nameof(expiryDate))
            .IsGreaterThan(issueDate);

        return new DriversLicense(
            normalizedLicenseNumber,
            normalizedIssueCountry,
            issueDate,
            expiryDate);
    }

    /// <summary>
    ///     Checks if the license is currently valid (not expired).
    /// </summary>
    public bool IsValid()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return ExpiryDate >= today;
    }

    /// <summary>
    ///     Checks if the license will be valid on a specific date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    public bool IsValidOn(DateOnly date) => date >= IssueDate && date <= ExpiryDate;

    /// <summary>
    ///     Checks if the license is from an EU member state.
    ///     EU licenses are generally recognized across all member states.
    /// </summary>
    public bool IsEuLicense()
    {
        var euCountries = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Germany",
            "Deutschland",
            "France",
            "Italy",
            "Spain",
            "Netherlands",
            "Belgium",
            "Austria",
            "Poland",
            "Portugal",
            "Greece",
            "Sweden",
            "Denmark",
            "Finland",
            "Ireland",
            "Czech Republic",
            "Romania",
            "Hungary",
            "Slovakia",
            "Bulgaria",
            "Croatia",
            "Slovenia",
            "Lithuania",
            "Latvia",
            "Estonia",
            "Luxembourg",
            "Malta",
            "Cyprus"
        };

        return euCountries.Contains(IssueCountry);
    }

    /// <summary>
    ///     Gets the number of days until the license expires.
    ///     Returns negative number if already expired.
    /// </summary>
    public int DaysUntilExpiry()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return ExpiryDate.DayNumber - today.DayNumber;
    }

    /// <summary>
    ///     Creates an anonymized driver's license for GDPR compliance.
    /// </summary>
    public static DriversLicense Anonymized()
    {
        return new DriversLicense(
            "ANONYMIZED000000",
            "Germany",
            new DateOnly(2000, 1, 1),
            new DateOnly(2099, 12, 31));
    }

    public override string ToString() => $"{LicenseNumber} ({IssueCountry}, expires {ExpiryDate:yyyy-MM-dd})";
}
