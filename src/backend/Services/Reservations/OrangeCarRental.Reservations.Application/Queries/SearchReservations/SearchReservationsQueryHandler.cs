using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Handler for SearchReservationsQuery.
///     Searches reservations with filters and returns paginated results.
/// </summary>
public sealed class SearchReservationsQueryHandler(IReservationRepository reservations)
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
            vehicleId,
            pickupDateFrom,
            pickupDateTo,
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
        return new ReservationDto
        {
            ReservationId = reservation.Id.Value,
            VehicleId = reservation.VehicleId,
            CustomerId = reservation.CustomerId,
            PickupDate = reservation.Period.PickupDate.ToDateTime(TimeOnly.MinValue),
            ReturnDate = reservation.Period.ReturnDate.ToDateTime(TimeOnly.MinValue),
            PickupLocationCode = reservation.PickupLocationCode.Value,
            DropoffLocationCode = reservation.DropoffLocationCode.Value,
            RentalDays = reservation.Period.Days,
            TotalPriceNet = reservation.TotalPrice.NetAmount,
            TotalPriceVat = reservation.TotalPrice.VatAmount,
            TotalPriceGross = reservation.TotalPrice.GrossAmount,
            Currency = reservation.TotalPrice.Currency.Code,
            Status = reservation.Status.ToString(),
            CancellationReason = reservation.CancellationReason,
            CreatedAt = reservation.CreatedAt,
            ConfirmedAt = reservation.ConfirmedAt,
            CancelledAt = reservation.CancelledAt,
            CompletedAt = reservation.CompletedAt
        };
    }
}
