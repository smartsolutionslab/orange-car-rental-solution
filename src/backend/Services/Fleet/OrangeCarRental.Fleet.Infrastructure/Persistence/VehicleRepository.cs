using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
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

        var query = Vehicles
            .AsNoTracking()
            .ApplyFilters(parameters);

        // Filter by date availability if period is provided (cross-service call)
        if (parameters.Period.HasValue)
        {
            var bookedVehicleIds = await reservationService.GetBookedVehicleIdsAsync(
                parameters.Period.Value,
                cancellationToken);

            if (bookedVehicleIds.Count > 0)
            {
                var bookedIdsSet = bookedVehicleIds.ToHashSet();
                query = query.Where(v => !bookedIdsSet.Contains(v.Id));
            }
        }

        query = query.ApplySorting(parameters.Sorting, SortFieldSelectors, v => v.Name);

        return await query.ToPagedResultAsync(parameters.Paging, cancellationToken);
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
    ///     Sort field selectors for vehicle queries.
    /// </summary>
    private static readonly Dictionary<string, Expression<Func<Vehicle, object?>>> SortFieldSelectors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = v => v.Name,
        ["category"] = v => v.Category,
        ["categorycode"] = v => v.Category,
        ["location"] = v => v.CurrentLocationCode,
        ["locationcode"] = v => v.CurrentLocationCode,
        ["seats"] = v => v.Seats,
        ["fueltype"] = v => v.FuelType,
        ["fuel"] = v => v.FuelType,
        ["transmissiontype"] = v => v.TransmissionType,
        ["transmission"] = v => v.TransmissionType,
        ["dailyrate"] = v => v.DailyRate.NetAmount + v.DailyRate.VatAmount,
        ["price"] = v => v.DailyRate.NetAmount + v.DailyRate.VatAmount,
        ["rate"] = v => v.DailyRate.NetAmount + v.DailyRate.VatAmount,
        ["status"] = v => v.Status
    };
}

/// <summary>
///     Filter extension methods for Vehicle queries.
/// </summary>
internal static class VehicleQueryExtensions
{
    /// <summary>
    ///     Applies all filters from VehicleSearchParameters to the query.
    /// </summary>
    public static IQueryable<Vehicle> ApplyFilters(
        this IQueryable<Vehicle> query,
        VehicleSearchParameters parameters)
    {
        return query
            .WhereIf(parameters.LocationCode.HasValue, v => v.CurrentLocationCode == parameters.LocationCode!.Value)
            .WhereIf(parameters.Category.HasValue, v => v.Category == parameters.Category!.Value)
            .WhereIf(parameters.MinSeats.HasValue, v => v.Seats >= parameters.MinSeats!.Value)
            .WhereIf(parameters.FuelType.HasValue, v => v.FuelType == parameters.FuelType!.Value)
            .WhereIf(parameters.TransmissionType.HasValue, v => v.TransmissionType == parameters.TransmissionType!.Value)
            .WhereIf(parameters.MaxDailyRate.HasValue, v =>
                v.DailyRate.NetAmount + v.DailyRate.VatAmount <=
                parameters.MaxDailyRate!.Value.NetAmount + parameters.MaxDailyRate.Value.VatAmount)
            .WhereIf(parameters.Status.HasValue, v => v.Status == parameters.Status!.Value);
    }
}
