namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

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
