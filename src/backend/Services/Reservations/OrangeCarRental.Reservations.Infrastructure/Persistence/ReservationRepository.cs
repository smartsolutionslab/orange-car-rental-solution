using Microsoft.EntityFrameworkCore;
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

    public async Task<(List<Reservation> Reservations, int TotalCount)> SearchAsync(
        ReservationStatus? status = null,
        CustomerIdentifier? customerId = null,
        string? customerName = null,
        VehicleIdentifier? vehicleId = null,
        string? categoryCode = null,
        string? pickupLocationCode = null,
        DateOnly? pickupDateFrom = null,
        DateOnly? pickupDateTo = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        string? sortBy = null,
        bool sortDescending = false,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = context.Reservations.AsNoTracking();

        // Apply filters
        if (status != null)
            query = query.Where(r => r.Status == status);

        if (customerId.HasValue)
            query = query.Where(r => r.CustomerIdentifier == customerId.Value);

        // Note: customerName filter requires denormalized customer data (not yet implemented)

        if (vehicleId.HasValue)
            query = query.Where(r => r.VehicleIdentifier == vehicleId.Value);

        // Note: categoryCode filter requires denormalized vehicle category data (not yet implemented)

        if (!string.IsNullOrWhiteSpace(pickupLocationCode))
            query = query.Where(r => r.PickupLocationCode.Value == pickupLocationCode);

        if (pickupDateFrom.HasValue)
            query = query.Where(r => r.Period.PickupDate >= pickupDateFrom.Value);

        if (pickupDateTo.HasValue)
            query = query.Where(r => r.Period.PickupDate <= pickupDateTo.Value);

        // Price range filters - access GrossAmount from Money complex property
        if (priceMin.HasValue)
            query = query.Where(r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount >= priceMin.Value);

        if (priceMax.HasValue)
            query = query.Where(r => r.TotalPrice.NetAmount + r.TotalPrice.VatAmount <= priceMax.Value);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, sortBy, sortDescending);

        // Apply pagination
        var reservations = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reservations, totalCount);
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
