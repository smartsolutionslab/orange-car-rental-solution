using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Handler for ConfirmReservationCommand.
///     Confirms a pending reservation via event sourcing after payment has been received.
/// </summary>
public sealed class ConfirmReservationCommandHandler(
    IEventSourcedReservationRepository repository)
    : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    public async Task<ConfirmReservationResult> HandleAsync(
        ConfirmReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load reservation from event store
        var reservation = await repository.LoadAsync(command.ReservationId, cancellationToken);
        if (!reservation.State.HasBeenCreated)
        {
            throw new InvalidOperationException($"Reservation with ID '{command.ReservationId.Value}' not found.");
        }

        // Execute domain logic
        reservation.Confirm();

        // Persist events to event store
        await repository.SaveAsync(reservation, cancellationToken);

        return new ConfirmReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.ConfirmedAt!.Value
        );
    }
}
