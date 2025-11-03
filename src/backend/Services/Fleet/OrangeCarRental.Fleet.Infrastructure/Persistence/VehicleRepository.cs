using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
/// Entity Framework implementation of IVehicleRepository.
/// </summary>
public sealed class VehicleRepository(FleetDbContext context, ReservationsDbContext reservationsContext) : IVehicleRepository
{
    public async Task<Vehicle?> GetByIdAsync(VehicleIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
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
        if (parameters.Category.HasValue)
        {
            query = query.Where(v => v.Category == parameters.Category.Value);
        }

        // Minimum seats filter
        if (parameters.MinSeats.HasValue)
        {
            query = query.Where(v => v.Seats >= SeatingCapacity.Of(parameters.MinSeats.Value));
        }

        // Fuel type filter
        if (parameters.FuelType.HasValue)
        {
            query = query.Where(v => v.FuelType == parameters.FuelType.Value);
        }

        // Transmission type filter
        if (parameters.TransmissionType.HasValue)
        {
            query = query.Where(v => v.TransmissionType == parameters.TransmissionType.Value);
        }

        // Max daily rate filter
        if (parameters.MaxDailyRateGross.HasValue)
        {
            query = query.Where(v =>
                v.DailyRate.NetAmount + v.DailyRate.VatAmount <= parameters.MaxDailyRateGross.Value);
        }

        // Status filter
        if (parameters.Status.HasValue)
        {
            query = query.Where(v => v.Status == parameters.Status.Value);
        }

        // Filter by date availability if both dates provided
        if (parameters.PickupDate.HasValue && parameters.ReturnDate.HasValue)
        {
            var pickupDate = DateOnly.FromDateTime(parameters.PickupDate.Value);
            var returnDate = DateOnly.FromDateTime(parameters.ReturnDate.Value);

            // Get booked vehicle IDs
            var bookedVehicleIds = await reservationsContext.Reservations
                .Where(r =>
                    (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Active) &&
                    r.Period.PickupDate <= returnDate &&
                    r.Period.ReturnDate >= pickupDate)
                .Select(r => r.VehicleId)
                .ToListAsync(cancellationToken);

            var bookedIdsSet = bookedVehicleIds.ToHashSet();

            // Get all matching vehicles and filter in memory
            var allVehicles = await query.ToListAsync(cancellationToken);
            var availableVehicles = allVehicles
                .Where(v => !bookedIdsSet.Contains(v.Id.Value))
                .ToList();

            // Apply pagination for in-memory collection
            return availableVehicles.ToPagedResult(parameters);
        }

        // Get total count and apply pagination for queryable
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

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default) => await context.Vehicles.AddAsync(vehicle, cancellationToken);

    public Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Update(vehicle);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(VehicleIdentifier id, CancellationToken cancellationToken = default)
    {
        var vehicle = await GetByIdAsync(id, cancellationToken);
        if (vehicle != null)
        {
            context.Vehicles.Remove(vehicle);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken);
}
