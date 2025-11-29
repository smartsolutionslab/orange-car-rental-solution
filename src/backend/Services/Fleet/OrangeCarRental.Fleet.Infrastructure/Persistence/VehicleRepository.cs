using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of IVehicleRepository.
/// </summary>
public sealed class VehicleRepository(FleetDbContext context, IReservationService reservationService)
    : IVehicleRepository
{
    private DbSet<Vehicle> Vehicles => context.Vehicles;

    public async Task<Vehicle> GetByIdAsync(
        VehicleIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await Vehicles.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle ?? throw new EntityNotFoundException(typeof(Vehicle), id);
    }

    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Vehicles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Vehicle>> SearchAsync(
        VehicleSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        parameters.Validate();

        // Start with base query
        var query = Vehicles.AsNoTracking().AsQueryable();

        // Apply filters using value objects directly - no parsing in repository

        // Location filter - use LocationCode directly
        if (parameters.LocationCode.HasValue)
        {
            query = query.Where(v => v.CurrentLocationCode == parameters.LocationCode.Value);
        }

        // Category filter - use value object directly
        if (parameters.Category.HasValue) query = query.Where(v => v.Category == parameters.Category.Value);

        // Minimum seats filter
        if (parameters.MinSeats.HasValue)
            query = query.Where(v => v.Seats >= parameters.MinSeats.Value);

        // Fuel type filter
        if (parameters.FuelType.HasValue) query = query.Where(v => v.FuelType == parameters.FuelType.Value);

        // Transmission type filter
        if (parameters.TransmissionType.HasValue)
            query = query.Where(v => v.TransmissionType == parameters.TransmissionType.Value);

        // Max daily rate filter
        if (parameters.MaxDailyRate.HasValue)
        {
            var maxRate = parameters.MaxDailyRate.Value;
            query = query.Where(v =>
                v.DailyRate.NetAmount + v.DailyRate.VatAmount <= maxRate.NetAmount + maxRate.VatAmount);
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

        // Apply sorting
        query = ApplySorting(query, parameters.Sorting.SortBy, parameters.Sorting.Descending);

        // Get total count and apply pagination at database level for all queries
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(parameters.Paging.Skip)
            .Take(parameters.Paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Vehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.Paging.PageNumber,
            PageSize = parameters.Paging.PageSize
        };
    }

    public async Task AddAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken = default) =>
        await Vehicles.AddAsync(vehicle, cancellationToken);

    public Task UpdateAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken = default)
    {
        Vehicles.Update(vehicle);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(
        VehicleIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await Vehicles.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vehicle != null)
        {
            Vehicles.Remove(vehicle);
        }
    }

    /// <summary>
    ///     Applies sorting to the query based on the specified field and direction.
    /// </summary>
    private static IQueryable<Vehicle> ApplySorting(
        IQueryable<Vehicle> query,
        string? sortBy,
        bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            // Default sorting: by name ascending
            return query.OrderBy(v => v.Name);
        }

        var sortField = sortBy.Trim().ToLowerInvariant();

        return sortField switch
        {
            "name" =>
                sortDescending ? query.OrderByDescending(v => v.Name) : query.OrderBy(v => v.Name),

            "category" or "categorycode" =>
                sortDescending ? query.OrderByDescending(v => v.Category) : query.OrderBy(v => v.Category),

            "location" or "locationcode" =>
                sortDescending
                    ? query.OrderByDescending(v => v.CurrentLocationCode)
                    : query.OrderBy(v => v.CurrentLocationCode),

            "seats" =>
                sortDescending ? query.OrderByDescending(v => v.Seats) : query.OrderBy(v => v.Seats),

            "fueltype" or "fuel" =>
                sortDescending ? query.OrderByDescending(v => v.FuelType) : query.OrderBy(v => v.FuelType),

            "transmissiontype" or "transmission" =>
                sortDescending
                    ? query.OrderByDescending(v => v.TransmissionType)
                    : query.OrderBy(v => v.TransmissionType),

            "dailyrate" or "price" or "rate" =>
                sortDescending
                    ? query.OrderByDescending(v => v.DailyRate.NetAmount + v.DailyRate.VatAmount)
                    : query.OrderBy(v => v.DailyRate.NetAmount + v.DailyRate.VatAmount),

            "status" =>
                sortDescending ? query.OrderByDescending(v => v.Status) : query.OrderBy(v => v.Status),

            // Default: sort by name
            _ => query.OrderBy(v => v.Name)
        };
    }
}
