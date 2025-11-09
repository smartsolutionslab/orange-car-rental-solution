using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Customer name value object with optional salutation.
///     Represents a complete customer name with German market conventions.
/// </summary>
public readonly record struct CustomerName
{
    /// <summary>
    ///     The customer's first name.
    /// </summary>
    public FirstName FirstName { get; init; }

    /// <summary>
    ///     The customer's last name.
    /// </summary>
    public LastName LastName { get; init; }

    /// <summary>
    ///     Optional salutation (e.g., Herr, Frau, Dr.).
    /// </summary>
    public Salutation? Salutation { get; init; }

    /// <summary>
    ///     Gets the full name without salutation.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    ///     Gets the formal name with salutation.
    ///     Example: "Herr Dr. Max Mustermann"
    /// </summary>
    public string FormalName => Salutation.HasValue
        ? $"{Salutation.Value} {FirstName} {LastName}"
        : FullName;

    /// <summary>
    ///     Creates a customer name value object.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="salutation">Optional salutation.</param>
    public static CustomerName Of(
        string firstName,
        string lastName,
        Salutation? salutation = null)
    {
        return new CustomerName
        {
            FirstName = FirstName.Of(firstName),
            LastName = LastName.Of(lastName),
            Salutation = salutation
        };
    }

    /// <summary>
    ///     Creates a customer name value object from FirstName and LastName value objects.
    /// </summary>
    public static CustomerName Of(
        FirstName firstName,
        LastName lastName,
        Salutation? salutation = null)
    {
        return new CustomerName
        {
            FirstName = firstName,
            LastName = lastName,
            Salutation = salutation
        };
    }

    /// <summary>
    ///     Creates an anonymized customer name for GDPR compliance.
    /// </summary>
    public static CustomerName Anonymized() => new()
    {
        FirstName = FirstName.Anonymized(),
        LastName = LastName.Anonymized(),
        Salutation = null
    };

    public override string ToString() => FormalName;
}
