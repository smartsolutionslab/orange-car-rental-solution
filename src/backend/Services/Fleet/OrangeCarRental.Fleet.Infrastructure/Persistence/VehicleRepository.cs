using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Enums;
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
        // Start with base query
        var query = context.Vehicles.AsNoTracking().AsQueryable();

        // Apply filters using database-level WHERE clauses
        // Use EF.Property to access the stored column values directly since value objects
        // with value converters cannot have their nested properties accessed in LINQ queries
        if (!string.IsNullOrWhiteSpace(parameters.LocationCode))
        {
            // Compare the value object directly - EF Core will use the value converter
            var location = Location.FromCode(parameters.LocationCode);
            query = query.Where(v => v.CurrentLocation == location);
        }

        if (!string.IsNullOrWhiteSpace(parameters.CategoryCode))
        {
            // Compare the value object directly - EF Core will use the value converter
            var category = VehicleCategory.FromCode(parameters.CategoryCode);
            query = query.Where(v => v.Category == category);
        }

        if (parameters.MinSeats.HasValue)
        {
            // Compare using the >= operator defined on SeatingCapacity
            // Use the comparison directly in the query for EF Core to translate it properly
            query = query.Where(v => v.Seats >= SeatingCapacity.Of(parameters.MinSeats.Value));
        }

        if (parameters.FuelType.HasValue)
        {
            query = query.Where(v => v.FuelType == parameters.FuelType.Value);
        }

        if (parameters.TransmissionType.HasValue)
        {
            query = query.Where(v => v.TransmissionType == parameters.TransmissionType.Value);
        }

        // Filter by MaxDailyRateGross using complex property members
        if (parameters.MaxDailyRateGross.HasValue)
        {
            // Access complex property members directly - EF Core can translate this
            query = query.Where(v =>
                v.DailyRate.NetAmount + v.DailyRate.VatAmount <= parameters.MaxDailyRateGross.Value);
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(v => v.Status == parameters.Status.Value);
        }

        // Get total count before date filtering and pagination
        var totalCountBeforeDateFilter = await query.CountAsync(cancellationToken);

        // Filter by date availability - exclude vehicles with overlapping reservations
        List<Vehicle> items;
        int totalCount;

        if (parameters.PickupDate.HasValue && parameters.ReturnDate.HasValue)
        {
            var pickupDate = parameters.PickupDate.Value.Date;
            var returnDate = parameters.ReturnDate.Value.Date;

            // Get vehicle IDs that have confirmed or active reservations overlapping with the requested period
            var bookedVehicleIds = await reservationsContext.Reservations
                .Where(r =>
                    (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Active) &&
                    r.Period.PickupDate <= returnDate &&
                    r.Period.ReturnDate >= pickupDate)
                .Select(r => r.VehicleId)
                .ToListAsync(cancellationToken);

            // Convert to HashSet for efficient lookup
            var bookedIdsSet = bookedVehicleIds.ToHashSet();

            // Get all matching vehicles and filter in memory
            var allVehicles = await query.ToListAsync(cancellationToken);

            // Filter out booked vehicles
            var availableVehicles = allVehicles
                .Where(v => !bookedIdsSet.Contains(v.Id.Value))
                .ToList();

            totalCount = availableVehicles.Count;

            // Apply pagination in memory
            items = availableVehicles
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList();
        }
        else
        {
            totalCount = totalCountBeforeDateFilter;

            // Apply pagination
            items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);
        }

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
