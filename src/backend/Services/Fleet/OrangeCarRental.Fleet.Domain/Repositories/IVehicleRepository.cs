using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;

/// <summary>
/// Repository interface for Vehicle aggregate.
/// </summary>
public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(VehicleIdentifier id, CancellationToken cancellationToken = default);

    Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    Task DeleteAsync(VehicleIdentifier id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
