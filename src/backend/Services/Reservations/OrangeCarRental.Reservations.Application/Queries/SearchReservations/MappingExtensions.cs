using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Extension methods for mapping between domain objects and DTOs.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Reservation mapping.
    /// </summary>
    extension(Reservation reservation)
    {
        /// <summary>
        ///     Maps a Reservation aggregate to a ReservationDto.
        /// </summary>
        public ReservationDto ToDto() => new(
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
            reservation.CompletedAt);
    }

    /// <summary>
    ///     C# 14 Extension Members for PagedResult of Reservation mapping.
    /// </summary>
    extension(PagedResult<Reservation> pagedResult)
    {
        /// <summary>
        ///     Maps a PagedResult of Reservation aggregates to a SearchReservationsResult.
        /// </summary>
        public SearchReservationsResult ToDto() => new(
            pagedResult.Items.Select(r => r.ToDto()).ToList(),
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalPages);
    }

    /// <summary>
    ///     C# 14 Extension Members for SearchReservationsQuery mapping.
    /// </summary>
    extension(SearchReservationsQuery query)
    {
        /// <summary>
        ///     Maps a SearchReservationsQuery to ReservationSearchParameters.
        ///     Handles parsing of primitive types to value objects.
        /// </summary>
        public ReservationSearchParameters ToSearchParameters()
        {
            var (status, customerId, customerName, vehicleId,
                category, pickupLocation, dateRange, priceRange,
                sortBy, sortDescending, pageNumber, pageSize) = query;

            return new ReservationSearchParameters(
                status,
                customerId,
                customerName,
                vehicleId,
                category,
                pickupLocation,
                dateRange,
                priceRange,
                PagingInfo.Create(pageNumber, pageSize),
                SortingInfo.Create(sortBy, sortDescending));
        }
    }
}
