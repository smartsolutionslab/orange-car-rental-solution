using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of IVehicleRepository.
/// </summary>
public sealed class VehicleRepository(FleetDbContext context, IReservationService reservationService)
    : IVehicleRepository
{
    public async Task<Vehicle> GetByIdAsync(VehicleIdentifier id, CancellationToken cancellationToken = default)
    {
        var vehicle = await context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle ?? throw new EntityNotFoundException(typeof(Vehicle), id);
    }

    public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Vehicles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Vehicle>> SearchAsync(
        VehicleSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        parameters.Validate();

        // Start with base query
        var query = context.Vehicles.AsNoTracking().AsQueryable();

        // Apply filters using value objects directly - no parsing in repository

        // Location filter - use value object directly
        if (parameters.LocationCode.HasValue)
        {
            var location = Location.FromCode(parameters.LocationCode.Value);
            query = query.Where(v => v.CurrentLocation == location);
        }

        // Category filter - use value object directly
        if (parameters.Category.HasValue) query = query.Where(v => v.Category == parameters.Category.Value);

        // Minimum seats filter
        if (parameters.MinSeats.HasValue)
            query = query.Where(v => v.Seats >= SeatingCapacity.Of(parameters.MinSeats.Value));

        // Fuel type filter
        if (parameters.FuelType.HasValue) query = query.Where(v => v.FuelType == parameters.FuelType.Value);

        // Transmission type filter
        if (parameters.TransmissionType.HasValue)
            query = query.Where(v => v.TransmissionType == parameters.TransmissionType.Value);

        // Max daily rate filter
        if (parameters.MaxDailyRateGross.HasValue)
        {
            query = query.Where(v =>
                v.DailyRate.NetAmount + v.DailyRate.VatAmount <= parameters.MaxDailyRateGross.Value);
        }

        // Status filter
        if (parameters.Status.HasValue) query = query.Where(v => v.Status == parameters.Status.Value);

        // Filter by date availability if period is provided
        if (parameters.Period.HasValue)
        {
            var searchPeriod = parameters.Period.Value;

            // Get booked vehicle IDs via Reservations API (maintains bounded context boundaries)
            var bookedVehicleIds = await reservationService.GetBookedVehicleIdsAsync(
                searchPeriod,
                cancellationToken);

            // Apply exclusion filter at database level - NOT in memory
            if (bookedVehicleIds.Count > 0)
            {
                var bookedIdsSet = bookedVehicleIds.ToHashSet();
                query = query.Where(v => !bookedIdsSet.Contains(v.Id));
            }
        }

        // Get total count and apply pagination at database level for all queries
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Vehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default) =>
        await context.Vehicles.AddAsync(vehicle, cancellationToken);

    public Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Update(vehicle);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(VehicleIdentifier id, CancellationToken cancellationToken = default)
    {
        var vehicle = await context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vehicle != null)
        {
            context.Vehicles.Remove(vehicle);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
