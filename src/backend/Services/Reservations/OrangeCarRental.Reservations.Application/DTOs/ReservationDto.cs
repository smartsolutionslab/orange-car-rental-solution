namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;

/// <summary>
///     Data transfer object for reservation details.
/// </summary>
public sealed record ReservationDto(
    Guid ReservationId,
    Guid VehicleId,
    Guid CustomerId,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    string PickupLocationCode,
    string DropoffLocationCode,
    int RentalDays,
    decimal TotalPriceNet,
    decimal TotalPriceVat,
    decimal TotalPriceGross,
    string Currency,
    string Status,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? CancelledAt,
    DateTime? CompletedAt);
