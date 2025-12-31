using SmartSolutionsLab.OrangeCarRental.Reservations.Api.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Requests;

/// <summary>
///     Request DTO for creating a guest reservation.
///     Accepts primitives from HTTP requests and maps to CreateGuestReservationCommand with value objects.
/// </summary>
public sealed record CreateGuestReservationRequest(
    ReservationDetailsDto Reservation,
    CustomerDetailsDto Customer,
    AddressDetailsDto Address,
    DriversLicenseDetailsDto DriversLicense);
