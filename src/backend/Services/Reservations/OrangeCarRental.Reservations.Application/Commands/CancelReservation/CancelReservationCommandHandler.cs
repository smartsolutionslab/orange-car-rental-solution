using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
///     Handler for CancelReservationCommand.
///     Cancels a reservation via repository with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IReservationRepository repository,
    IReservationsUnitOfWork unitOfWork)
    : ICommandHandler<CancelReservationCommand, CancelReservationResult>
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load reservation from repository
        var reservation = await repository.GetByIdAsync(command.ReservationId, cancellationToken);

        // Execute domain logic
        reservation.Cancel(command.CancellationReason);

        // Persist changes
        await repository.UpdateAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.CancelledAt!.Value,
            reservation.CancellationReason
        );
    }
}
