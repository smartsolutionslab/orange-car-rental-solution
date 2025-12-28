using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;

/// <summary>
///     Handler for CancelReservationCommand.
///     Cancels a reservation via event sourcing with an optional reason.
/// </summary>
public sealed class CancelReservationCommandHandler(
    IEventSourcedReservationRepository repository)
    : ICommandHandler<CancelReservationCommand, CancelReservationResult>
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load reservation from event store
        var reservation = await repository.LoadAsync(command.ReservationId, cancellationToken);
        if (!reservation.State.HasBeenCreated)
        {
            throw new InvalidOperationException($"Reservation with ID '{command.ReservationId.Value}' not found.");
        }

        // Execute domain logic
        reservation.Cancel(command.CancellationReason);

        // Persist events to event store
        await repository.SaveAsync(reservation, cancellationToken);

        return new CancelReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.CancelledAt!.Value,
            reservation.CancellationReason
        );
    }
}
