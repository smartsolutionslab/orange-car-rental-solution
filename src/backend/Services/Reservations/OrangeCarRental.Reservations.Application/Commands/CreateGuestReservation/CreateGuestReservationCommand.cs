namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;

/// <summary>
///     Command to create a reservation for a guest (user without prior registration).
///     This command handles both customer registration and reservation creation in a single transaction.
/// </summary>
public sealed record CreateGuestReservationCommand
{
    // Vehicle and Reservation Details
    public required Guid VehicleId { get; init; }
    public required string CategoryCode { get; init; }
    public required DateTime PickupDate { get; init; }
    public required DateTime ReturnDate { get; init; }
    public required string PickupLocationCode { get; init; }
    public required string DropoffLocationCode { get; init; }

    // Customer Details (for inline registration)
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required DateOnly DateOfBirth { get; init; }

    // Address Details
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public string Country { get; init; } = "Germany";

    // Driver's License Details
    public required string LicenseNumber { get; init; }
    public required string LicenseIssueCountry { get; init; }
    public required DateOnly LicenseIssueDate { get; init; }
    public required DateOnly LicenseExpiryDate { get; init; }
}
