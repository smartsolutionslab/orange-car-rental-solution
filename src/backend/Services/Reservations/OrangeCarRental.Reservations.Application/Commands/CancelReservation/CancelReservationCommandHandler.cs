using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
///     Handler for CancelReservationCommand.
///     Cancels a reservation with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IReservationRepository reservations)
    : ICommandHandler<CancelReservationCommand, CancelReservationResult>
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get existing reservation
        var reservation = await reservations.GetByIdAsync(command.ReservationId, cancellationToken) ?? throw new InvalidOperationException($"Reservation {command.ReservationId} not found");

        // Cancel the reservation (returns new instance due to immutability)
        var cancelledReservation = reservation.Cancel(command.CancellationReason);

        // Update in repository
        await reservations.UpdateAsync(cancelledReservation, cancellationToken);
        await reservations.SaveChangesAsync(cancellationToken);

        return new CancelReservationResult(
            cancelledReservation.Id.Value,
            cancelledReservation.Status.ToString(),
            cancelledReservation.CancelledAt!.Value,
            cancelledReservation.CancellationReason
        );
    }
}
