using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
///     Filter extension methods for Reservation queries.
/// </summary>
internal static class ReservationQueryExtensions
{
    /// <summary>
    ///     Applies all filters from ReservationSearchParameters to the query.
    /// </summary>
    public static IQueryable<Reservation> ApplyFilters(
        this IQueryable<Reservation> query,
        ReservationSearchParameters parameters)
    {
        query = query
            .WhereIf(parameters.Status != null, r => r.Status == parameters.Status)
            .WhereIf(parameters.CustomerId.HasValue, r => r.CustomerIdentifier == parameters.CustomerId!.Value)
            .WhereIf(parameters.VehicleId.HasValue, r => r.VehicleIdentifier == parameters.VehicleId!.Value)
            .WhereIf(parameters.PickupLocationCode.HasValue, r => r.PickupLocationCode == parameters.PickupLocationCode!.Value);

        // Pickup date range filtering
        query = query.WhereInDateRange(
            parameters.PickupDateRange,
            r => r.Period.PickupDate >= parameters.PickupDateRange!.From!.Value,
            r => r.Period.PickupDate <= parameters.PickupDateRange!.To!.Value);

        // Price range filtering
        query = query.WhereInPriceRange(
            parameters.PriceRange,
            r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount >= parameters.PriceRange!.Min!.Value,
            r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount <= parameters.PriceRange!.Max!.Value);

        return query;
    }
}
