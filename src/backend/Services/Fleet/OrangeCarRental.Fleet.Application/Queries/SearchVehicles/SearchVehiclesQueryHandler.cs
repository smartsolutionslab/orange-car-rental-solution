using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Handler for SearchVehiclesQuery.
/// Searches and filters vehicles based on query parameters.
/// </summary>
public sealed class SearchVehiclesQueryHandler
{
    // In a real implementation, this would use IVehicleReadRepository
    // For now, we'll return sample data to demonstrate the structure

    public Task<SearchVehiclesResult> HandleAsync(
        SearchVehiclesQuery query,
        CancellationToken cancellationToken = default)
    {
        // TODO: Replace with actual database query
        // For now, return sample vehicles to demonstrate the API
        var vehicles = GetSampleVehicles();

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

        var result = new SearchVehiclesResult
        {
            Vehicles = dtos,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };

        return Task.FromResult(result);
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

    // Sample data for demonstration - TODO: Remove when database is implemented
    private List<Vehicle> GetSampleVehicles()
    {
        var vehicles = new List<Vehicle>();

        // Kleinwagen in Berlin
        var golf = Vehicle.From(
            VehicleName.Of("VW Golf"),
            VehicleCategory.Kompaktklasse,
            Location.BerlinHauptbahnhof,
            Money.Euro(45m), // 45€ net, 53.55€ gross per day
            SeatingCapacity.Of(5),
            FuelType.Petrol,
            TransmissionType.Manual
        );
        golf.SetDetails(Manufacturer.Of("Volkswagen"), VehicleModel.Of("Golf"), ManufacturingYear.Of(2023), "https://example.com/golf.jpg");
        vehicles.Add(golf);

        // Mittelklasse in München
        var bmw3 = Vehicle.From(
            VehicleName.Of("BMW 3er"),
            VehicleCategory.Mittelklasse,
            Location.MunichFlughafen,
            Money.Euro(75m), // 75€ net, 89.25€ gross per day
            SeatingCapacity.Of(5),
            FuelType.Diesel,
            TransmissionType.Automatic
        );
        bmw3.SetDetails(Manufacturer.Of("BMW"), VehicleModel.Of("3er"), ManufacturingYear.Of(2024), "https://example.com/bmw3.jpg");
        vehicles.Add(bmw3);

        // SUV in Frankfurt
        var x5 = Vehicle.From(
            VehicleName.Of("BMW X5"),
            VehicleCategory.SUV,
            Location.FrankfurtFlughafen,
            Money.Euro(120m), // 120€ net, 142.80€ gross per day
            SeatingCapacity.Of(7),
            FuelType.Diesel,
            TransmissionType.Automatic
        );
        x5.SetDetails(Manufacturer.Of("BMW"), VehicleModel.Of("X5"), ManufacturingYear.Of(2024), "https://example.com/x5.jpg");
        vehicles.Add(x5);

        // Elektro in Hamburg
        var id3 = Vehicle.From(
            VehicleName.Of("VW ID.3"),
            VehicleCategory.Kompaktklasse,
            Location.HamburgHauptbahnhof,
            Money.Euro(55m), // 55€ net, 65.45€ gross per day
            SeatingCapacity.Of(5),
            FuelType.Electric,
            TransmissionType.Automatic
        );
        id3.SetDetails(Manufacturer.Of("Volkswagen"), VehicleModel.Of("ID.3"), ManufacturingYear.Of(2024), "https://example.com/id3.jpg");
        vehicles.Add(id3);

        // Oberklasse in Köln
        var mercedes = Vehicle.From(
            VehicleName.Of("Mercedes E-Klasse"),
            VehicleCategory.Oberklasse,
            Location.KolnHauptbahnhof,
            Money.Euro(95m), // 95€ net, 113.05€ gross per day
            SeatingCapacity.Of(5),
            FuelType.Hybrid,
            TransmissionType.Automatic
        );
        mercedes.SetDetails(Manufacturer.Of("Mercedes-Benz"), VehicleModel.Of("E-Klasse"), ManufacturingYear.Of(2024), "https://example.com/mercedes-e.jpg");
        vehicles.Add(mercedes);

        return vehicles;
    }
}
