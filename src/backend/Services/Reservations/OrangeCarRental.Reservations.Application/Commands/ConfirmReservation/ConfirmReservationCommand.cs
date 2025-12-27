using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
///     Command to confirm a pending reservation (payment received).
/// </summary>
public sealed record ConfirmReservationCommand(
    ReservationIdentifier ReservationId)
    : ICommand<ConfirmReservationResult>;
