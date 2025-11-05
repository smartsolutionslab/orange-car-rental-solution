namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Command to register a new customer.
///     Contains all required customer registration data with German market validation.
/// </summary>
public sealed record RegisterCustomerCommand
{
    /// <summary>
    ///     Customer's first name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    ///     Customer's last name.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    ///     Customer's email address (will be normalized to lowercase).
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Customer's phone number (German format: +49 XXX XXXXXXX or 0XXX XXXXXXX).
    /// </summary>
    public required string PhoneNumber { get; init; }

    /// <summary>
    ///     Customer's date of birth.
    ///     Must be at least 18 years old.
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    ///     Street address (e.g., "Hauptstraße 123").
    /// </summary>
    public required string Street { get; init; }

    /// <summary>
    ///     City name (e.g., "Berlin").
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    ///     German postal code (5 digits, e.g., "10115").
    /// </summary>
    public required string PostalCode { get; init; }

    /// <summary>
    ///     Country name (defaults to "Germany" if not provided).
    /// </summary>
    public string Country { get; init; } = "Germany";

    /// <summary>
    ///     Driver's license number (alphanumeric).
    /// </summary>
    public required string LicenseNumber { get; init; }

    /// <summary>
    ///     Country that issued the driver's license (e.g., "Germany").
    /// </summary>
    public required string LicenseIssueCountry { get; init; }

    /// <summary>
    ///     Date the driver's license was issued.
    /// </summary>
    public required DateOnly LicenseIssueDate { get; init; }

    /// <summary>
    ///     Date the driver's license expires.
    ///     Must be valid for at least 30 days.
    /// </summary>
    public required DateOnly LicenseExpiryDate { get; init; }
}
