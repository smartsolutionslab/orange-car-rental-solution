namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Requests;

/// <summary>
///     Request DTO for creating a new reservation.
///     Accepts primitives from HTTP requests and maps to CreateReservationCommand with value objects.
/// </summary>
public sealed record CreateReservationRequest(
    Guid VehicleId,
    Guid CustomerId,
    string CategoryCode,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    string PickupLocationCode,
    string DropoffLocationCode,
    decimal? TotalPriceNet);
