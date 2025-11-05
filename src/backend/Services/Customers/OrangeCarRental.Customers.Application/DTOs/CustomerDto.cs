namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
///     Data transfer object for customer information.
///     Maps from Customer aggregate with all properties for display.
/// </summary>
public sealed record CustomerDto
{
    /// <summary>
    ///     Unique customer identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Customer's first name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    ///     Customer's last name.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    ///     Customer's full name (FirstName + LastName).
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    ///     Customer's email address (normalized to lowercase).
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Customer's phone number in normalized format (+49XXXXXXXXXX).
    /// </summary>
    public required string PhoneNumber { get; init; }

    /// <summary>
    ///     Customer's phone number in formatted display format (+49 XXX XXXXXXX).
    /// </summary>
    public required string PhoneNumberFormatted { get; init; }

    /// <summary>
    ///     Customer's date of birth.
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    ///     Customer's current age in years.
    /// </summary>
    public required int Age { get; init; }

    /// <summary>
    ///     Customer's address information.
    /// </summary>
    public required AddressDto Address { get; init; }

    /// <summary>
    ///     Customer's driver's license information.
    /// </summary>
    public required DriversLicenseDto DriversLicense { get; init; }

    /// <summary>
    ///     Current account status (Active, Suspended, Blocked).
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    ///     Indicates if the customer can make reservations.
    /// </summary>
    public required bool CanMakeReservation { get; init; }

    /// <summary>
    ///     Date and time when the customer registered (UTC).
    /// </summary>
    public required DateTime RegisteredAtUtc { get; init; }

    /// <summary>
    ///     Date and time when the customer profile was last updated (UTC).
    /// </summary>
    public required DateTime UpdatedAtUtc { get; init; }
}
