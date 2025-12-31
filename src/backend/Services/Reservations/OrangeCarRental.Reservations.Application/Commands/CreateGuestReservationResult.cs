namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Result of creating a guest reservation.
///     Contains both the newly created customer ID and reservation ID.
/// </summary>
public sealed record CreateGuestReservationResult(
    Guid CustomerId,
    Guid ReservationId,
    decimal TotalPriceNet,
    decimal TotalPriceVat,
    decimal TotalPriceGross,
    string Currency
);
