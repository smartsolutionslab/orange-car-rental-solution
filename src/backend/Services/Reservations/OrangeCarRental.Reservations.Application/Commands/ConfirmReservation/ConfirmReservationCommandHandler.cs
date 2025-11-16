using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Handler for ConfirmReservationCommand.
///     Confirms a pending reservation after payment has been received.
/// </summary>
public sealed class ConfirmReservationCommandHandler(
    IReservationRepository reservations)
    : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    public async Task<ConfirmReservationResult> HandleAsync(
        ConfirmReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get existing reservation (throws EntityNotFoundException if not found)
        var reservation = await reservations.GetByIdAsync(command.ReservationId, cancellationToken);

        // Confirm the reservation (returns new instance due to immutability)
        var confirmedReservation = reservation.Confirm();

        // Update in repository
        await reservations.UpdateAsync(confirmedReservation, cancellationToken);
        await reservations.SaveChangesAsync(cancellationToken);

        return new ConfirmReservationResult(
            confirmedReservation.Id.Value,
            confirmedReservation.Status.ToString(),
            confirmedReservation.ConfirmedAt!.Value
        );
    }
}
