using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries;

/// <summary>
///     Handler for GetReservationQuery.
///     Retrieves a reservation by ID and maps to DTO.
/// </summary>
public sealed class GetReservationQueryHandler(IReservationRepository reservations)
    : IQueryHandler<GetReservationQuery, ReservationDto>
{
    public async Task<ReservationDto> HandleAsync(
        GetReservationQuery query,
        CancellationToken cancellationToken = default)
    {
        var reservation = await reservations.GetByIdAsync(query.ReservationId, cancellationToken);

        return reservation.ToDto();
    }
}
