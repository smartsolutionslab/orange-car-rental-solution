namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Command to confirm a pending reservation (payment received).
/// </summary>
public sealed record ConfirmReservationCommand(Guid ReservationId);
