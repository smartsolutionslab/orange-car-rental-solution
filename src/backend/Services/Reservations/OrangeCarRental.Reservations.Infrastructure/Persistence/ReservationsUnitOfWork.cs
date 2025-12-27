using System.Diagnostics.CodeAnalysis;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Reservations bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class ReservationsUnitOfWork(ReservationsDbContext context) : IReservationsUnitOfWork
{
    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IReservationRepository Reservations =>
        field ??= new ReservationRepository(context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
