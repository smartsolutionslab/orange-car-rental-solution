using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Handler for ConfirmReservationCommand.
///     Confirms a pending reservation via event sourcing after payment has been received.
/// </summary>
public sealed class ConfirmReservationCommandHandler(
    IReservationCommandService commandService)
    : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    public async Task<ConfirmReservationResult> HandleAsync(
        ConfirmReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Confirm the reservation via event-sourced command service
        var reservation = await commandService.ConfirmAsync(
            command.ReservationId,
            cancellationToken);

        return new ConfirmReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.ConfirmedAt!.Value
        );
    }
}
