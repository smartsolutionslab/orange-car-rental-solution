using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
///     Handler for CancelReservationCommand.
///     Cancels a reservation via event sourcing with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IReservationCommandService commandService)
    : ICommandHandler<CancelReservationCommand, CancelReservationResult>
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Cancel the reservation via event-sourced command service
        var reservation = await commandService.CancelAsync(
            command.ReservationId,
            command.CancellationReason,
            cancellationToken);

        return new CancelReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.CancelledAt!.Value,
            reservation.CancellationReason
        );
    }
}
