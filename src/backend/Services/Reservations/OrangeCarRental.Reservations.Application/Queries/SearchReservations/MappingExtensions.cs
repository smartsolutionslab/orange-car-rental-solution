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
        ///     Uses .Value to access nullable struct properties.
        /// </summary>
        public ReservationDto ToDto() => new(
            reservation.Id.Value,
            reservation.VehicleIdentifier!.Value.Value,
            reservation.CustomerIdentifier!.Value.Value,
            reservation.Period!.Value.PickupDate,
            reservation.Period!.Value.ReturnDate,
            reservation.PickupLocationCode!.Value.Value,
            reservation.DropoffLocationCode!.Value.Value,
            reservation.Period!.Value.Days,
            reservation.TotalPrice!.Value.NetAmount,
            reservation.TotalPrice!.Value.VatAmount,
            reservation.TotalPrice!.Value.GrossAmount,
            reservation.TotalPrice!.Value.Currency.Code,
            reservation.Status.ToString(),
            reservation.CancellationReason,
            reservation.CreatedAt,
            reservation.ConfirmedAt,
            reservation.CancelledAt,
            reservation.CompletedAt);
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
