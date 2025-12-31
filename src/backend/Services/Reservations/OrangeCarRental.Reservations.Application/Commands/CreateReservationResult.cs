namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Result of creating a reservation.
/// </summary>
public sealed record CreateReservationResult(
    Guid ReservationId,
    string Status,
    decimal TotalPriceNet,
    decimal TotalPriceVat,
    decimal TotalPriceGross
);
