namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Command to update a customer's profile information.
///     Does not include email or driver's license (use separate commands for those).
/// </summary>
public sealed record UpdateCustomerProfileCommand
{
    /// <summary>
    ///     The unique identifier of the customer to update.
    /// </summary>
    public required Guid CustomerIdentifier { get; init; }

    /// <summary>
    ///     Customer's updated first name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    ///     Customer's updated last name.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    ///     Customer's updated phone number (German format: +49 XXX XXXXXXX or 0XXX XXXXXXX).
    /// </summary>
    public required string PhoneNumber { get; init; }

    /// <summary>
    ///     Updated street address (e.g., "Hauptstraße 123").
    /// </summary>
    public required string Street { get; init; }

    /// <summary>
    ///     Updated city name (e.g., "Berlin").
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    ///     Updated German postal code (5 digits, e.g., "10115").
    /// </summary>
    public required string PostalCode { get; init; }

    /// <summary>
    ///     Updated country name (defaults to "Germany" if not provided).
    /// </summary>
    public string Country { get; init; } = "Germany";
}
