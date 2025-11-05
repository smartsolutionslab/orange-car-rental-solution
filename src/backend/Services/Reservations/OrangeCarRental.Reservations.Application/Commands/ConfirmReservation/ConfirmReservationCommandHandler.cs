using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
/// Handler for ConfirmReservationCommand.
/// Confirms a pending reservation after payment has been received.
/// </summary>
public sealed class ConfirmReservationCommandHandler(
    IReservationRepository repository)
{
    public async Task<ConfirmReservationResult> HandleAsync(
        ConfirmReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get existing reservation
        var reservationId = ReservationIdentifier.From(command.ReservationId);
        var reservation = await repository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation {command.ReservationId} not found");
        }

        // Confirm the reservation (returns new instance due to immutability)
        var confirmedReservation = reservation.Confirm();

        // Update in repository
        await repository.UpdateAsync(confirmedReservation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new ConfirmReservationResult(
            confirmedReservation.Id.Value,
            confirmedReservation.Status.ToString(),
            confirmedReservation.ConfirmedAt!.Value
        );
    }
}
