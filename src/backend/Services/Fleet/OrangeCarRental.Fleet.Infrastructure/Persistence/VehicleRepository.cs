using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
/// Entity Framework implementation of IVehicleRepository.
/// </summary>
public sealed class VehicleRepository : IVehicleRepository
{
    private readonly FleetDbContext _context;

    public VehicleRepository(FleetDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Vehicle?> GetByIdAsync(VehicleIdentifier id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Vehicle>> SearchAsync(
        VehicleSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        // Start with base query
        var query = _context.Vehicles.AsNoTracking().AsQueryable();

        // Apply filters using database-level WHERE clauses
        if (!string.IsNullOrWhiteSpace(parameters.LocationCode))
        {
            query = query.Where(v => v.CurrentLocation.Code == parameters.LocationCode);
        }

        if (!string.IsNullOrWhiteSpace(parameters.CategoryCode))
        {
            query = query.Where(v => v.Category.Code == parameters.CategoryCode);
        }

        if (parameters.MinSeats.HasValue)
        {
            query = query.Where(v => v.Seats.Value >= parameters.MinSeats.Value);
        }

        if (parameters.FuelType.HasValue)
        {
            query = query.Where(v => v.FuelType == parameters.FuelType.Value);
        }

        if (parameters.TransmissionType.HasValue)
        {
            query = query.Where(v => v.TransmissionType == parameters.TransmissionType.Value);
        }

        if (parameters.MaxDailyRateGross.HasValue)
        {
            query = query.Where(v => v.DailyRate.GrossAmount <= parameters.MaxDailyRateGross.Value);
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(v => v.Status == parameters.Status.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Vehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(vehicle, cancellationToken);
    }

    public Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        _context.Vehicles.Update(vehicle);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(VehicleIdentifier id, CancellationToken cancellationToken = default)
    {
        var vehicle = await GetByIdAsync(id, cancellationToken);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
