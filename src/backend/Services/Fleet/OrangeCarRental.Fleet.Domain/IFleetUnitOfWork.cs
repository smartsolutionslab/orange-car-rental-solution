using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain;

/// <summary>
///     Unit of Work for the Fleet bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface IFleetUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the vehicle repository.
    /// </summary>
    IVehicleRepository Vehicles { get; }

    /// <summary>
    ///     Gets the location repository.
    /// </summary>
    ILocationRepository Locations { get; }
}
