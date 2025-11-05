using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
/// Handler for CancelReservationCommand.
/// Cancels a reservation with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IReservationRepository repository)
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get existing reservation
        var reservationId = ReservationIdentifier.From(command.ReservationId);
        var reservation = await repository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation {command.ReservationId} not found");
        }

        // Cancel the reservation (returns new instance due to immutability)
        var cancelledReservation = reservation.Cancel(command.CancellationReason);

        // Update in repository
        await repository.UpdateAsync(cancelledReservation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new CancelReservationResult(
            cancelledReservation.Id.Value,
            cancelledReservation.Status.ToString(),
            cancelledReservation.CancelledAt!.Value,
            cancelledReservation.CancellationReason
        );
    }
}
