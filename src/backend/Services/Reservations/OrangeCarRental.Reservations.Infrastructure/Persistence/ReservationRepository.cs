using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
/// Entity Framework implementation of IReservationRepository.
/// </summary>
public sealed class ReservationRepository(ReservationsDbContext context) : IReservationRepository
{
    public async Task<Reservation?> GetByIdAsync(ReservationIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Reservations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Reservation> Reservations, int TotalCount)> SearchAsync(
        ReservationStatus? status = null,
        Guid? customerId = null,
        Guid? vehicleId = null,
        DateOnly? pickupDateFrom = null,
        DateOnly? pickupDateTo = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = context.Reservations.AsNoTracking();

        // Apply filters
        if (status != null)
        {
            query = query.Where(r => r.Status == status);
        }

        if (customerId.HasValue)
        {
            query = query.Where(r => r.CustomerId == customerId.Value);
        }

        if (vehicleId.HasValue)
        {
            query = query.Where(r => r.VehicleId == vehicleId.Value);
        }

        if (pickupDateFrom.HasValue)
        {
            query = query.Where(r => r.Period.PickupDate >= pickupDateFrom.Value);
        }

        if (pickupDateTo.HasValue)
        {
            query = query.Where(r => r.Period.PickupDate <= pickupDateTo.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var reservations = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reservations, totalCount);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        context.Reservations.Update(reservation);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(ReservationIdentifier id, CancellationToken cancellationToken = default)
    {
        var reservation = await GetByIdAsync(id, cancellationToken);
        if (reservation != null)
        {
            context.Reservations.Remove(reservation);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
