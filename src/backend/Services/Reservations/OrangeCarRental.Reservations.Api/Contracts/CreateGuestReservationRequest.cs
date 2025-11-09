namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Contracts;

/// <summary>
///     Request DTO for creating a guest reservation.
///     Accepts primitives from HTTP requests and maps to CreateGuestReservationCommand with value objects.
/// </summary>
public sealed record CreateGuestReservationRequest(
    ReservationDetailsDto Reservation,
    CustomerDetailsDto Customer,
    AddressDetailsDto Address,
    DriversLicenseDetailsDto DriversLicense);

/// <summary>
///     Vehicle and reservation details.
/// </summary>
public sealed record ReservationDetailsDto
{
    public required Guid VehicleId { get; init; }
    public required string CategoryCode { get; init; }
    public required DateTime PickupDate { get; init; }
    public required DateTime ReturnDate { get; init; }
    public required string PickupLocationCode { get; init; }
    public required string DropoffLocationCode { get; init; }
}

/// <summary>
///     Customer details for inline registration.
/// </summary>
public sealed record CustomerDetailsDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required DateOnly DateOfBirth { get; init; }
}

/// <summary>
///     Address details.
/// </summary>
public sealed record AddressDetailsDto
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public string Country { get; init; } = "Germany";
}

/// <summary>
///     Driver's license details.
/// </summary>
public sealed record DriversLicenseDetailsDto
{
    public required string LicenseNumber { get; init; }
    public required string LicenseIssueCountry { get; init; }
    public required DateOnly LicenseIssueDate { get; init; }
    public required DateOnly LicenseExpiryDate { get; init; }
}
