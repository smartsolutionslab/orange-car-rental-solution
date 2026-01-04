using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Handler for CancelReservationCommand.
///     Cancels a reservation with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IReservationRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CancelReservationCommand, CancelReservationResult>
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load reservation from database
        var reservation = await repository.GetByIdAsync(command.ReservationId, cancellationToken);

        // Execute domain logic (returns new instance - immutable pattern)
        var cancelledReservation = reservation.Cancel(command.CancellationReason);

        // Persist changes to database
        await repository.UpdateAsync(cancelledReservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelReservationResult(
            cancelledReservation.Id.Value,
            cancelledReservation.Status.ToString(),
            cancelledReservation.CancelledAt!.Value,
            cancelledReservation.CancellationReason
        );
    }
}
