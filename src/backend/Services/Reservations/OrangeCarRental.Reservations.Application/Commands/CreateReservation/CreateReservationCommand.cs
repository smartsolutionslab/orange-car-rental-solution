namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

/// <summary>
/// Command to create a new reservation.
/// </summary>
public sealed record CreateReservationCommand(
    Guid VehicleId,
    Guid CustomerId,
    DateTime PickupDate,
    DateTime ReturnDate,
    string PickupLocationCode,
    string DropoffLocationCode,
    decimal TotalPriceNet
);
