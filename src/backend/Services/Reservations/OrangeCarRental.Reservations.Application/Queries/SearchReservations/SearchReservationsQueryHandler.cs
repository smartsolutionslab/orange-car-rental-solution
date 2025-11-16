using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

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
        // Parse status if provided
        ReservationStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ReservationStatus>(query.Status, ignoreCase: true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        // Convert DateTime to DateOnly for pickup date filters
        DateOnly? pickupDateFrom = query.PickupDateFrom.HasValue
            ? DateOnly.FromDateTime(query.PickupDateFrom.Value)
            : null;

        DateOnly? pickupDateTo = query.PickupDateTo.HasValue
            ? DateOnly.FromDateTime(query.PickupDateTo.Value)
            : null;

        // Extract Guid values from value objects for repository query
        Guid? customerId = query.CustomerId?.Value;
        Guid? vehicleId = query.VehicleId?.Value;

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
            reservation.VehicleId.Value,
            reservation.CustomerId.Value,
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
