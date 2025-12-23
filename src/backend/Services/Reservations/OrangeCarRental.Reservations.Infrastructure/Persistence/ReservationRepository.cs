using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of IReservationRepository.
/// </summary>
public sealed class ReservationRepository(ReservationsDbContext context) : IReservationRepository
{
    private DbSet<Reservation> Reservations => context.Reservations;
    public async Task<Reservation> GetByIdAsync(
        ReservationIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var reservation = await Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return reservation ?? throw new EntityNotFoundException(typeof(Reservation), id);
    }

    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Reservations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Reservation>> SearchAsync(
        ReservationSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = Reservations
            .AsNoTracking()
            .ApplyFilters(parameters)
            .ApplySorting(parameters.Sorting, SortFieldSelectors, r => r.CreatedAt, defaultDescending: true);

        return await query.ToPagedResultAsync(parameters.Paging, cancellationToken);
    }

    /// <summary>
    ///     Sort field selectors for reservation queries.
    ///     Note: .Value used to access nullable struct properties in EF Core queries.
    /// </summary>
    private static readonly Dictionary<string, Expression<Func<Reservation, object?>>> SortFieldSelectors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pickupdate"] = r => r.Period!.Value.PickupDate,
        ["pickup_date"] = r => r.Period!.Value.PickupDate,
        ["price"] = r => r.TotalPrice!.Value.NetAmount + r.TotalPrice!.Value.VatAmount,
        ["totalprice"] = r => r.TotalPrice!.Value.NetAmount + r.TotalPrice!.Value.VatAmount,
        ["total_price"] = r => r.TotalPrice!.Value.NetAmount + r.TotalPrice!.Value.VatAmount,
        ["status"] = r => r.Status,
        ["createddate"] = r => r.CreatedAt,
        ["created_date"] = r => r.CreatedAt,
        ["createdat"] = r => r.CreatedAt,
        ["created_at"] = r => r.CreatedAt
    };

    public async Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default) =>
        await Reservations.AddAsync(reservation, cancellationToken);

    /// <summary>
    ///     Marks the reservation for update in the DbContext.
    ///     Note: EF Core's Update() is synchronous. Changes are persisted when SaveChangesAsync() is called.
    /// </summary>
    public Task UpdateAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        Reservations.Update(reservation);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(
        ReservationIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var reservation = await Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reservation != null)
        {
            Reservations.Remove(reservation);
        }
    }

    public async Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
       BookingPeriod period,
        CancellationToken cancellationToken = default)
    {
        // Get all reservations that overlap with the requested period
        // A reservation is considered "booked" if it's Confirmed or Active
        var bookedVehicleIds = await Reservations
            .AsNoTracking()
            .Where(r =>
                (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Active) &&
                // Period overlap check: reservation period overlaps if:
                // reservation pickup <= requested return AND reservation return >= requested pickup
                r.Period!.Value.PickupDate <= period.ReturnDate &&
                r.Period!.Value.ReturnDate >= period.PickupDate)
            .Select(r => r.VehicleIdentifier!.Value.Value) // Extract Guid from nullable struct
            .Distinct()
            .ToListAsync(cancellationToken);

        return bookedVehicleIds;
    }
}

/// <summary>
///     Filter extension methods for Reservation queries.
/// </summary>
internal static class ReservationQueryExtensions
{
    /// <summary>
    ///     Applies all filters from ReservationSearchParameters to the query.
    ///     Note: .Value used to access nullable struct properties in EF Core queries.
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
            r => r.Period!.Value.PickupDate >= parameters.PickupDateRange!.From!.Value,
            r => r.Period!.Value.PickupDate <= parameters.PickupDateRange!.To!.Value);

        // Price range filtering
        query = query.WhereInPriceRange(
            parameters.PriceRange,
            r => r.TotalPrice!.Value.NetAmount + r.TotalPrice!.Value.VatAmount >= parameters.PriceRange!.Min!.Value,
            r => r.TotalPrice!.Value.NetAmount + r.TotalPrice!.Value.VatAmount <= parameters.PriceRange!.Max!.Value);

        return query;
    }
}
