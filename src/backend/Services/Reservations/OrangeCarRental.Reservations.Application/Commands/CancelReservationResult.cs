namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Result of cancelling a reservation.
/// </summary>
public sealed record CancelReservationResult(
    Guid ReservationId,
    string Status,
    DateTime CancelledAt,
    string? CancellationReason
);
