using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Handler for SearchVehiclesQuery.
/// Searches and filters vehicles based on query parameters.
/// </summary>
public sealed class SearchVehiclesQueryHandler
{
    private readonly IVehicleRepository _repository;

    public SearchVehiclesQueryHandler(IVehicleRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<SearchVehiclesResult> HandleAsync(
        SearchVehiclesQuery query,
        CancellationToken cancellationToken = default)
    {
        // Get all vehicles from database
        var vehicles = await _repository.GetAllAsync(cancellationToken);

        // Apply filters
        var filtered = ApplyFilters(vehicles, query);

        // Apply pagination with defaults
        var pageNumber = query.PageNumber ?? 1;
        var pageSize = query.PageSize ?? 20;

        var total = filtered.Count;
        var paged = filtered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Map to DTOs
        var dtos = paged.Select(MapToDto).ToList();

        return new SearchVehiclesResult
        {
            Vehicles = dtos,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    private List<Vehicle> ApplyFilters(List<Vehicle> vehicles, SearchVehiclesQuery query)
    {
        var filtered = vehicles.AsEnumerable();

        // Filter by location
        if (!string.IsNullOrWhiteSpace(query.LocationCode))
        {
            filtered = filtered.Where(v =>
                v.CurrentLocation.Code.Equals(query.LocationCode, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by category
        if (!string.IsNullOrWhiteSpace(query.CategoryCode))
        {
            filtered = filtered.Where(v =>
                v.Category.Code.Equals(query.CategoryCode, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by minimum seats
        if (query.MinSeats.HasValue)
        {
            filtered = filtered.Where(v => v.Seats.Value >= query.MinSeats.Value);
        }

        // Filter by fuel type
        if (!string.IsNullOrWhiteSpace(query.FuelType))
        {
            if (Enum.TryParse<FuelType>(query.FuelType, ignoreCase: true, out var fuelType))
            {
                filtered = filtered.Where(v => v.FuelType == fuelType);
            }
        }

        // Filter by transmission type
        if (!string.IsNullOrWhiteSpace(query.TransmissionType))
        {
            if (Enum.TryParse<TransmissionType>(query.TransmissionType, ignoreCase: true, out var transmissionType))
            {
                filtered = filtered.Where(v => v.TransmissionType == transmissionType);
            }
        }

        // Filter by maximum daily rate (gross)
        if (query.MaxDailyRateGross.HasValue)
        {
            filtered = filtered.Where(v => v.DailyRate.GrossAmount <= query.MaxDailyRateGross.Value);
        }

        // Filter by availability status
        filtered = filtered.Where(v => v.Status == VehicleStatus.Available);

        return filtered.ToList();
    }

    private VehicleDto MapToDto(Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id.Value,
            Name = vehicle.Name.Value,
            CategoryCode = vehicle.Category.Code,
            CategoryName = vehicle.Category.Name,
            LocationCode = vehicle.CurrentLocation.Code,
            City = vehicle.CurrentLocation.City,
            Seats = vehicle.Seats.Value,
            FuelType = vehicle.FuelType.ToString(),
            TransmissionType = vehicle.TransmissionType.ToString(),
            DailyRateNet = vehicle.DailyRate.NetAmount,
            DailyRateVat = vehicle.DailyRate.VatAmount,
            DailyRateGross = vehicle.DailyRate.GrossAmount,
            Currency = vehicle.DailyRate.Currency.Code,
            Status = vehicle.Status.ToString(),
            LicensePlate = vehicle.LicensePlate,
            Manufacturer = vehicle.Manufacturer?.Value,
            Model = vehicle.Model?.Value,
            Year = vehicle.Year?.Value,
            ImageUrl = vehicle.ImageUrl
        };
    }
}
