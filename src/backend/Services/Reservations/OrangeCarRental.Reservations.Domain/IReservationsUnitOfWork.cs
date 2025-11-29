using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain;

/// <summary>
///     Unit of Work for the Reservations bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface IReservationsUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the reservation repository.
    /// </summary>
    IReservationRepository Reservations { get; }
}
