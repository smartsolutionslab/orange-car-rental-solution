using SmartSolutionsLab.OrangeCarRental.Reservations.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Reservations bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class ReservationsUnitOfWork : IReservationsUnitOfWork
{
    private readonly ReservationsDbContext _context;
    private IReservationRepository? _reservations;

    public ReservationsUnitOfWork(ReservationsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IReservationRepository Reservations =>
        _reservations ??= new ReservationRepository(_context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
