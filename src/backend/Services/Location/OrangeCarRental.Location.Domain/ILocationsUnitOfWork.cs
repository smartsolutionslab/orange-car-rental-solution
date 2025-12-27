using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain;

/// <summary>
///     Unit of Work for the Locations bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface ILocationsUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the location repository.
    /// </summary>
    ILocationRepository Locations { get; }
}
