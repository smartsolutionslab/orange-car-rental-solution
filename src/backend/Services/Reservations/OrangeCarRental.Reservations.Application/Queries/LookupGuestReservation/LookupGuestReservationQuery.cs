using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.LookupGuestReservation;

/// <summary>
///     Query to lookup a guest reservation by ID and email.
///     Used for anonymous users to find their reservation without authentication.
/// </summary>
public sealed record LookupGuestReservationQuery(
    ReservationIdentifier ReservationId,
    string Email
) : IQuery<ReservationDto>;
