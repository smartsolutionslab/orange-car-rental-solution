using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;
using CustomerIdentifier = SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared.CustomerIdentifier;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Handler for SearchReservationsQuery.
///     Searches reservations with filters and returns paginated results.
/// </summary>
public sealed class SearchReservationsQueryHandler(IReservationRepository reservations)
    : IQueryHandler<SearchReservationsQuery, SearchReservationsResult>
{
    public async Task<SearchReservationsResult> HandleAsync(
        SearchReservationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var parameters = new ReservationSearchParameters(
            Status: query.Status.TryParseReservationStatus(),
            CustomerId: CustomerIdentifier.From(query.CustomerId),
            CustomerName: query.CustomerName,
            VehicleId: VehicleIdentifier.From(query.VehicleId),
            CategoryCode: query.CategoryCode,
            PickupLocationCode: query.PickupLocationCode,
            PickupDateFrom: query.PickupDateFrom,
            PickupDateTo: query.PickupDateTo,
            PriceMin: query.PriceMin,
            PriceMax: query.PriceMax,
            SortBy: query.SortBy,
            SortDescending: query.SortDescending,
            PageNumber: query.PageNumber,
            PageSize: query.PageSize
        );

        // Search reservations using the unified search parameters
        var pagedResult = await reservations.SearchAsync(parameters, cancellationToken);

        // Map to DTOs using the PagedResult.Map extension
        return new SearchReservationsResult(
            pagedResult.Items.Select(MapToDto).ToList(),
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize
        );
    }

    private static ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto(
            reservation.Id.Value,
            reservation.VehicleIdentifier.Value,
            reservation.CustomerIdentifier.Value,
            reservation.Period.PickupDate,
            reservation.Period.ReturnDate,
            reservation.PickupLocationCode.Value,
            reservation.DropoffLocationCode.Value,
            reservation.Period.Days,
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount,
            reservation.TotalPrice.Currency.Code,
            reservation.Status.ToString(),
            reservation.CancellationReason,
            reservation.CreatedAt,
            reservation.ConfirmedAt,
            reservation.CancelledAt,
            reservation.CompletedAt
        );
    }
}
