using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Command to cancel a reservation.
/// </summary>
public sealed record CancelReservationCommand(
    ReservationIdentifier ReservationId,
    string? CancellationReason = null
) : ICommand<CancelReservationResult>;
