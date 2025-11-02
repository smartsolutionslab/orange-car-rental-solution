namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

/// <summary>
/// Driver's license value object.
/// Represents a driver's license with validation for EU/German requirements.
/// </summary>
public readonly record struct DriversLicense
{
    public string LicenseNumber { get; }
    public string IssueCountry { get; }
    public DateOnly IssueDate { get; }
    public DateOnly ExpiryDate { get; }

    private DriversLicense(string licenseNumber, string issueCountry, DateOnly issueDate, DateOnly expiryDate)
    {
        LicenseNumber = licenseNumber;
        IssueCountry = issueCountry;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
    }

    /// <summary>
    /// Creates a driver's license value object.
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
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number cannot be empty", nameof(licenseNumber));

        if (string.IsNullOrWhiteSpace(issueCountry))
            throw new ArgumentException("Issue country cannot be empty", nameof(issueCountry));

        var normalizedLicenseNumber = licenseNumber.Trim().ToUpperInvariant();
        var normalizedIssueCountry = issueCountry.Trim();

        // Validate license number format (alphanumeric, spaces allowed)
        if (!normalizedLicenseNumber.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
        {
            throw new ArgumentException(
                "License number must contain only letters, digits, and spaces",
                nameof(licenseNumber));
        }

        // Validate length
        if (normalizedLicenseNumber.Length < 5 || normalizedLicenseNumber.Length > 20)
        {
            throw new ArgumentException(
                "License number must be between 5 and 20 characters",
                nameof(licenseNumber));
        }

        // Validate dates
        if (issueDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ArgumentException(
                "License issue date cannot be in the future",
                nameof(issueDate));
        }

        if (expiryDate <= issueDate)
        {
            throw new ArgumentException(
                "License expiry date must be after issue date",
                nameof(expiryDate));
        }

        // Validate that license was issued at least 18 years after a reasonable birth year
        // (This is a sanity check - actual age validation is done in the Customer aggregate)
        var minimumIssueDate = new DateOnly(1950, 1, 1);
        if (issueDate < minimumIssueDate)
        {
            throw new ArgumentException(
                $"License issue date cannot be before {minimumIssueDate}",
                nameof(issueDate));
        }

        // Validate country
        if (normalizedIssueCountry.Length > 100)
        {
            throw new ArgumentException(
                "Issue country name is too long (max 100 characters)",
                nameof(issueCountry));
        }

        return new DriversLicense(
            normalizedLicenseNumber,
            normalizedIssueCountry,
            issueDate,
            expiryDate);
    }

    /// <summary>
    /// Checks if the license is currently valid (not expired).
    /// </summary>
    public bool IsValid()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return ExpiryDate >= today;
    }

    /// <summary>
    /// Checks if the license will be valid on a specific date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    public bool IsValidOn(DateOnly date)
    {
        return date >= IssueDate && date <= ExpiryDate;
    }

    /// <summary>
    /// Checks if the license is from an EU member state.
    /// EU licenses are generally recognized across all member states.
    /// </summary>
    public bool IsEuLicense()
    {
        var euCountries = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Germany", "Deutschland", "France", "Italy", "Spain", "Netherlands",
            "Belgium", "Austria", "Poland", "Portugal", "Greece", "Sweden",
            "Denmark", "Finland", "Ireland", "Czech Republic", "Romania",
            "Hungary", "Slovakia", "Bulgaria", "Croatia", "Slovenia",
            "Lithuania", "Latvia", "Estonia", "Luxembourg", "Malta", "Cyprus"
        };

        return euCountries.Contains(IssueCountry);
    }

    /// <summary>
    /// Gets the number of days until the license expires.
    /// Returns negative number if already expired.
    /// </summary>
    public int DaysUntilExpiry()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return ExpiryDate.DayNumber - today.DayNumber;
    }

    /// <summary>
    /// Creates an anonymized driver's license for GDPR compliance.
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
