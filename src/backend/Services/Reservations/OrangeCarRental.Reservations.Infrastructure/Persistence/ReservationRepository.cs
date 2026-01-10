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

    public IAsyncEnumerable<Reservation> StreamAllAsync(CancellationToken cancellationToken = default)
    {
        return Reservations
            .AsNoTracking()
            .AsAsyncEnumerable();
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
    /// </summary>
    private static readonly Dictionary<string, Expression<Func<Reservation, object?>>> SortFieldSelectors = new(StringComparer.OrdinalIgnoreCase)
    {
        [ReservationSortFields.PickupDate] = r => r.Period.PickupDate,
        [ReservationSortFields.PickupDateAlt] = r => r.Period.PickupDate,
        [ReservationSortFields.Price] = r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount,
        [ReservationSortFields.TotalPrice] = r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount,
        [ReservationSortFields.TotalPriceAlt] = r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount,
        [ReservationSortFields.Status] = r => r.Status,
        [ReservationSortFields.CreatedDate] = r => r.CreatedAt,
        [ReservationSortFields.CreatedDateAlt] = r => r.CreatedAt,
        [ReservationSortFields.CreatedAt] = r => r.CreatedAt,
        [ReservationSortFields.CreatedAtAlt] = r => r.CreatedAt
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

        if (reservation != null) Reservations.Remove(reservation);
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
                r.Period.PickupDate <= period.ReturnDate &&
                r.Period.ReturnDate >= period.PickupDate)
            .Select(r => r.VehicleIdentifier.Value) // Extract Guid from value object
            .Distinct()
            .ToListAsync(cancellationToken);

        return bookedVehicleIds;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
