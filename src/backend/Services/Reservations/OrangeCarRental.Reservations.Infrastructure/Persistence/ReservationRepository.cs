using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

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
