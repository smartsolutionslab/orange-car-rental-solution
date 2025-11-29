namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Contracts;

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