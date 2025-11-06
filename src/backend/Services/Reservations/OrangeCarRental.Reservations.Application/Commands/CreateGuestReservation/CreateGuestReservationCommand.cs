using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;

/// <summary>
///     Command to create a reservation for a guest (user without prior registration).
///     This command handles both customer registration and reservation creation in a single transaction.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record CreateGuestReservationCommand
{
    // Vehicle and Reservation Details
    public required Guid VehicleId { get; init; }
    public required string CategoryCode { get; init; }
    public required BookingPeriod Period { get; init; }
    public required LocationCode PickupLocationCode { get; init; }
    public required LocationCode DropoffLocationCode { get; init; }

    // Customer Details (for inline registration)
    // Using value objects from Customers.Domain for validation and type safety
    public required FirstName FirstName { get; init; }
    public required LastName LastName { get; init; }
    public required Email Email { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
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
