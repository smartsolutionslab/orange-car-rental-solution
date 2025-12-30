namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Result of confirming a reservation.
/// </summary>
public sealed record ConfirmReservationResult(
    Guid ReservationId,
    string Status,
    DateTime ConfirmedAt
);
