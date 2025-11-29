using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Fleet bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class FleetUnitOfWork(FleetDbContext context, IReservationService reservationService)
    : IFleetUnitOfWork
{
    private IVehicleRepository? vehicles;
    private ILocationRepository? locations;

    /// <inheritdoc />
    public IVehicleRepository Vehicles =>
        vehicles ??= new VehicleRepository(context, reservationService);

    /// <inheritdoc />
    public ILocationRepository Locations =>
        locations ??= new LocationRepository(context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
