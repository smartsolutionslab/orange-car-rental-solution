using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Handler for SearchReservationsQuery.
///     Searches reservations with filters and returns paginated results.
/// </summary>
public sealed class SearchReservationsQueryHandler(IReservationRepository reservations)
    : IQueryHandler<SearchReservationsQuery, PagedResult<ReservationDto>>
{
    public async Task<PagedResult<ReservationDto>> HandleAsync(
        SearchReservationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var parameters = query.ToSearchParameters();
        var pagedResult = await reservations.SearchAsync(parameters, cancellationToken);

        return pagedResult.Map(reservation => reservation.ToDto());
    }
}
