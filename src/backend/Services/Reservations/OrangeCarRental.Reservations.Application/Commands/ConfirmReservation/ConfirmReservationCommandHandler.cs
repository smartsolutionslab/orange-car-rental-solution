using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Handler for ConfirmReservationCommand.
///     Confirms a pending reservation via repository after payment has been received.
/// </summary>
public sealed class ConfirmReservationCommandHandler(
    IReservationRepository repository,
    IReservationsUnitOfWork unitOfWork)
    : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    public async Task<ConfirmReservationResult> HandleAsync(
        ConfirmReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load reservation from repository
        var reservation = await repository.GetByIdAsync(command.ReservationId, cancellationToken);

        // Execute domain logic
        reservation.Confirm();

        // Persist changes
        await repository.UpdateAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.ConfirmedAt!.Value
        );
    }
}
