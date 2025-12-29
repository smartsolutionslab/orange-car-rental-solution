using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Handler for ConfirmReservationCommand.
///     Confirms a pending reservation after payment has been received.
/// </summary>
public sealed class ConfirmReservationCommandHandler(
    IReservationRepository repository)
    : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    public async Task<ConfirmReservationResult> HandleAsync(
        ConfirmReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load reservation from database
        var reservation = await repository.GetByIdAsync(command.ReservationId, cancellationToken);

        // Execute domain logic (returns new instance - immutable pattern)
        var confirmedReservation = reservation.Confirm();

        // Persist changes to database
        await repository.UpdateAsync(confirmedReservation, cancellationToken);

        return new ConfirmReservationResult(
            confirmedReservation.Id.Value,
            confirmedReservation.Status.ToString(),
            confirmedReservation.ConfirmedAt!.Value
        );
    }
}
