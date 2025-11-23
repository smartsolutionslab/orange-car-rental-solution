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
public sealed record ReservationDetailsDto(
    Guid VehicleId,
    string CategoryCode,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    string PickupLocationCode,
    string DropoffLocationCode);

/// <summary>
///     Customer details for inline registration.
/// </summary>
public sealed record CustomerDetailsDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth);

/// <summary>
///     Address details.
/// </summary>
public sealed record AddressDetailsDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");

/// <summary>
///     Driver's license details.
/// </summary>
public sealed record DriversLicenseDetailsDto(
    string LicenseNumber,
    string LicenseIssueCountry,
    DateOnly LicenseIssueDate,
    DateOnly LicenseExpiryDate);
