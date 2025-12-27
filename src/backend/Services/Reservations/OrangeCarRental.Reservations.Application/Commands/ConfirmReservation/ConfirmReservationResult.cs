namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Result of confirming a reservation.
/// </summary>
public sealed record ConfirmReservationResult(
    Guid ReservationId,
    string Status,
    DateTime ConfirmedAt
);
