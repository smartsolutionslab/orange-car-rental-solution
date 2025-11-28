using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of IReservationRepository.
/// </summary>
public sealed class ReservationRepository(ReservationsDbContext context) : IReservationRepository
{
    public async Task<Reservation> GetByIdAsync(ReservationIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var reservation = await context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return reservation ?? throw new EntityNotFoundException(typeof(Reservation), id);
    }

    public async Task<List<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Reservations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Reservation>> SearchAsync(
        ReservationSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = context.Reservations.AsNoTracking();

        // Apply filters
        if (parameters.Status != null)
            query = query.Where(r => r.Status == parameters.Status);

        if (parameters.CustomerId.HasValue)
            query = query.Where(r => r.CustomerIdentifier == parameters.CustomerId.Value);

        // Note: customerName filter requires denormalized customer data (not yet implemented)

        if (parameters.VehicleId.HasValue)
            query = query.Where(r => r.VehicleIdentifier == parameters.VehicleId.Value);

        // Note: categoryCode filter requires denormalized vehicle category data (not yet implemented)

        if (!string.IsNullOrWhiteSpace(parameters.PickupLocationCode))
            query = query.Where(r => r.PickupLocationCode.Value == parameters.PickupLocationCode);

        if (parameters.PickupDateFrom.HasValue)
            query = query.Where(r => r.Period.PickupDate >= parameters.PickupDateFrom.Value);

        if (parameters.PickupDateTo.HasValue)
            query = query.Where(r => r.Period.PickupDate <= parameters.PickupDateTo.Value);

        // Price range filters - access GrossAmount from Money complex property
        if (parameters.PriceMin.HasValue)
            query = query.Where(r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount >= parameters.PriceMin.Value);

        if (parameters.PriceMax.HasValue)
            query = query.Where(r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount <= parameters.PriceMax.Value);

        // Get total count before pagination (executed at database level)
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Apply pagination (uses Skip/Take properties from SearchParameters)
        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Reservation>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<Reservation> ApplySorting(
        IQueryable<Reservation> query,
        string? sortBy,
        bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "pickupdate" => sortDescending
                ? query.OrderByDescending(r => r.Period.PickupDate)
                : query.OrderBy(r => r.Period.PickupDate),

            "price" => sortDescending
                ? query.OrderByDescending(r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount)
                : query.OrderBy(r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount),

            "status" => sortDescending
                ? query.OrderByDescending(r => r.Status)
                : query.OrderBy(r => r.Status),

            "createddate" => sortDescending
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),

            // Default sorting by CreatedAt descending
            _ => query.OrderByDescending(r => r.CreatedAt)
        };
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default) =>
        await context.Reservations.AddAsync(reservation, cancellationToken);

    /// <summary>
    ///     Marks the reservation for update in the DbContext.
    ///     Note: EF Core's Update() is synchronous. Changes are persisted when SaveChangesAsync() is called.
    /// </summary>
    public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        context.Reservations.Update(reservation);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(ReservationIdentifier id, CancellationToken cancellationToken = default)
    {
        var reservation = await context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reservation != null)
        {
            context.Reservations.Remove(reservation);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
       BookingPeriod period,
        CancellationToken cancellationToken = default)
    {
        // Get all reservations that overlap with the requested period
        // A reservation is considered "booked" if it's Confirmed or Active
        var bookedVehicleIds = await context.Reservations
            .AsNoTracking()
            .Where(r =>
                (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Active) &&
                // Period overlap check: reservation period overlaps if:
                // reservation pickup <= requested return AND reservation return >= requested pickup
                r.Period.PickupDate <=  period.ReturnDate &&
                r.Period.ReturnDate >= period.PickupDate)
            .Select(r => r.VehicleIdentifier.Value) // Extract Guid from value object
            .Distinct()
            .ToListAsync(cancellationToken);

        return bookedVehicleIds;
    }
}
