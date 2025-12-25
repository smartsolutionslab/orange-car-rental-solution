using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Company name for business customers.
/// </summary>
public readonly record struct CompanyName : IValueObject
{
    /// <summary>
    ///     Maximum length for company name.
    /// </summary>
    public const int MaxLength = 200;

    /// <summary>
    ///     Minimum length for company name.
    /// </summary>
    public const int MinLength = 2;

    /// <summary>
    ///     Gets the company name value.
    /// </summary>
    public string Value { get; }

    private CompanyName(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a new company name.
    /// </summary>
    /// <param name="value">The company name.</param>
    /// <returns>A validated company name.</returns>
    /// <exception cref="ArgumentException">If the company name is invalid.</exception>
    public static CompanyName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Company name cannot be empty", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            throw new ArgumentException($"Company name must be at least {MinLength} characters", nameof(value));

        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"Company name cannot exceed {MaxLength} characters", nameof(value));

        return new CompanyName(trimmed);
    }

    /// <summary>
    ///     Gets whether this company name includes a German legal form suffix.
    /// </summary>
    public bool HasGermanLegalForm()
    {
        var legalForms = new[]
        {
            "GmbH", "AG", "KG", "OHG", "GbR", "UG", "eG", "e.V.", "e.K.",
            "GmbH & Co. KG", "GmbH & Co. OHG", "SE", "Ltd.", "KGaA"
        };

        var value = Value; // Copy to local to allow use in lambda
        return legalForms.Any(form =>
            value.EndsWith(form, StringComparison.OrdinalIgnoreCase) ||
            value.Contains($" {form} ", StringComparison.OrdinalIgnoreCase) ||
            value.Contains($" {form},", StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString() => Value;

    public static implicit operator string(CompanyName companyName) => companyName.Value;
}
