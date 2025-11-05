namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
/// Command to cancel a reservation.
/// </summary>
public sealed record CancelReservationCommand(
    Guid ReservationId,
    string? CancellationReason = null
);
