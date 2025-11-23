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

        var status = query.Status.TryParseReservationStatus();
        var pickupDateFrom = query.PickupDateFrom;
        var pickupDateTo = query.PickupDateTo;
        var customerId = CustomerIdentifier.From(query.CustomerId);
        var vehicleId = VehicleIdentifier.From(query.VehicleId);

        // Search reservations
        var (reservationsList, totalCount) = await reservations.SearchAsync(
            status,
            customerId,
            query.CustomerName,
            vehicleId,
            query.CategoryCode,
            query.PickupLocationCode,
            pickupDateFrom,
            pickupDateTo,
            query.PriceMin,
            query.PriceMax,
            query.SortBy,
            query.SortDescending,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        // Map to DTOs
        var reservationDtos = reservationsList.Select(MapToDto).ToList();

        return new SearchReservationsResult(
            reservationDtos,
            totalCount,
            query.PageNumber,
            query.PageSize
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
