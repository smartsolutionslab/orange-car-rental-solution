using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
///     Handler for CancelReservationCommand.
///     Cancels a reservation with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IReservationsUnitOfWork unitOfWork)
    : ICommandHandler<CancelReservationCommand, CancelReservationResult>
{
    private IReservationRepository? reservations;
    private IReservationRepository Reservations => reservations ??= unitOfWork.Reservations;
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var reservation = await Reservations.GetByIdAsync(command.ReservationId, cancellationToken);

        var cancelledReservation = reservation.Cancel(command.CancellationReason);
        await Reservations.UpdateAsync(cancelledReservation, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelReservationResult(
            cancelledReservation.Id.Value,
            cancelledReservation.Status.ToString(),
            cancelledReservation.CancelledAt!.Value,
            cancelledReservation.CancellationReason
        );
    }
}
