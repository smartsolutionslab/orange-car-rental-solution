using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Data;

/// <summary>
///     Seeds the Fleet database with sample locations and vehicles for development and testing.
///     Creates rental locations in major German cities and a diverse fleet of vehicles.
/// </summary>
public class FleetDataSeeder(
    FleetDbContext context,
    ILogger<FleetDataSeeder> logger)
{
    /// <summary>
    ///     Seeds the database with sample locations and vehicles if they don't exist.
    /// </summary>
    public async Task SeedAsync()
    {
        // Seed locations first
        await SeedLocationsAsync();

        // Then seed vehicles
        await SeedVehiclesAsync();
    }

    /// <summary>
    ///     Seeds the database with sample locations if no locations exist.
    /// </summary>
    private async Task SeedLocationsAsync()
    {
        var existingLocationCount = await context.Locations.CountAsync();
        if (existingLocationCount > 0)
        {
            logger.LogInformation("Fleet database already contains {Count} locations. Skipping location seed.", existingLocationCount);
            return;
        }

        logger.LogInformation("Seeding Fleet database with sample locations...");

        var locations = CreateSampleLocations();
        await context.Locations.AddRangeAsync(locations);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} locations to Fleet database.", locations.Count);
    }

    /// <summary>
    ///     Seeds the database with sample vehicles if no vehicles exist.
    /// </summary>
    private async Task SeedVehiclesAsync()
    {
        var existingVehicleCount = await context.Vehicles.CountAsync();
        if (existingVehicleCount > 0)
        {
            logger.LogInformation("Fleet database already contains {Count} vehicles. Skipping vehicle seed.", existingVehicleCount);
            return;
        }

        logger.LogInformation("Seeding Fleet database with sample vehicles...");

        var vehicles = CreateSampleVehicles();
        await context.Vehicles.AddRangeAsync(vehicles);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} vehicles to Fleet database.", vehicles.Count);
    }

    /// <summary>
    ///     Creates sample German rental locations.
    /// </summary>
    private List<Location> CreateSampleLocations()
    {
        return
        [
            Location.Create(
                LocationCode.Of("BER-HBF"),
                LocationName.Of("Berlin Hauptbahnhof"),
                Address.Of(
                    Street.Of("Europaplatz 1"),
                    City.Of("Berlin"),
                    PostalCode.Of("10557")
                )
            ),
            Location.Create(
                LocationCode.Of("MUC-FLG"),
                LocationName.Of("Munich Airport"),
                Address.Of(
                    Street.Of("Nordallee 25"),
                    City.Of("Munich"),
                    PostalCode.Of("85356")
                )
            ),
            Location.Create(
                LocationCode.Of("FRA-FLG"),
                LocationName.Of("Frankfurt Airport"),
                Address.Of(
                    Street.Of("Hugo-Eckener-Ring 1"),
                    City.Of("Frankfurt am Main"),
                    PostalCode.Of("60549")
                )
            ),
            Location.Create(
                LocationCode.Of("HAM-HBF"),
                LocationName.Of("Hamburg Hauptbahnhof"),
                Address.Of(
                    Street.Of("Hachmannplatz 16"),
                    City.Of("Hamburg"),
                    PostalCode.Of("20099")
                )
            ),
            Location.Create(
                LocationCode.Of("CGN-HBF"),
                LocationName.Of("Cologne Central Station"),
                Address.Of(
                    Street.Of("Trankgasse 11"),
                    City.Of("Cologne"),
                    PostalCode.Of("50667")
                )
            )
        ];
    }

    private List<Vehicle> CreateSampleVehicles()
    {
        var vehicles = new List<Vehicle>();

        // Kleinwagen (Small Cars) - 29-45 EUR/day
        vehicles.AddRange([
            CreateVehicle("VW Up!", VehicleCategory.Kleinwagen, LocationCode.Of("BER-HBF"),
                Money.Euro(29.99m), 4, FuelType.Petrol, TransmissionType.Manual,
                "Volkswagen", "Up!", 2023),
            CreateVehicle("Fiat 500", VehicleCategory.Kleinwagen, LocationCode.Of("MUC-FLG"),
                Money.Euro(32.99m), 4, FuelType.Petrol, TransmissionType.Manual,
                "Fiat", "500", 2024),
            CreateVehicle("Smart ForTwo", VehicleCategory.Kleinwagen, LocationCode.Of("FRA-FLG"),
                Money.Euro(34.99m), 2, FuelType.Electric, TransmissionType.Automatic,
                "Smart", "ForTwo EQ", 2023),
            CreateVehicle("Renault Twingo", VehicleCategory.Kleinwagen, LocationCode.Of("HAM-HBF"),
                Money.Euro(28.99m), 4, FuelType.Petrol, TransmissionType.Manual,
                "Renault", "Twingo", 2023),
            CreateVehicle("Toyota Aygo", VehicleCategory.Kleinwagen, LocationCode.Of("CGN-HBF"),
                Money.Euro(30.99m), 4, FuelType.Hybrid, TransmissionType.Automatic,
                "Toyota", "Aygo X", 2024)
        ]);

        // Kompaktklasse (Compact Cars) - 45-65 EUR/day
        vehicles.AddRange([
            CreateVehicle("VW Golf", VehicleCategory.Kompaktklasse, LocationCode.Of("BER-HBF"),
                Money.Euro(49.99m), 5, FuelType.Diesel, TransmissionType.Manual,
                "Volkswagen", "Golf 8", 2024),
            CreateVehicle("VW Golf Hybrid", VehicleCategory.Kompaktklasse, LocationCode.Of("MUC-FLG"),
                Money.Euro(59.99m), 5, FuelType.Hybrid, TransmissionType.Automatic,
                "Volkswagen", "Golf 8 GTE", 2024),
            CreateVehicle("Audi A3", VehicleCategory.Kompaktklasse, LocationCode.Of("FRA-FLG"),
                Money.Euro(62.99m), 5, FuelType.Petrol, TransmissionType.Automatic,
                "Audi", "A3 Sportback", 2024),
            CreateVehicle("BMW 1er", VehicleCategory.Kompaktklasse, LocationCode.Of("HAM-HBF"),
                Money.Euro(64.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "BMW", "118d", 2024),
            CreateVehicle("Mercedes A-Klasse", VehicleCategory.Kompaktklasse, LocationCode.Of("CGN-HBF"),
                Money.Euro(63.99m), 5, FuelType.Petrol, TransmissionType.Automatic,
                "Mercedes-Benz", "A 200", 2024),
            CreateVehicle("Opel Astra", VehicleCategory.Kompaktklasse, LocationCode.Of("BER-HBF"),
                Money.Euro(47.99m), 5, FuelType.Diesel, TransmissionType.Manual,
                "Opel", "Astra", 2023),
            CreateVehicle("Ford Focus", VehicleCategory.Kompaktklasse, LocationCode.Of("MUC-FLG"),
                Money.Euro(46.99m), 5, FuelType.Petrol, TransmissionType.Manual,
                "Ford", "Focus", 2023)
        ]);

        // Mittelklasse (Mid-Size Cars) - 65-95 EUR/day
        vehicles.AddRange([
            CreateVehicle("VW Passat", VehicleCategory.Mittelklasse, LocationCode.Of("BER-HBF"),
                Money.Euro(69.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "Volkswagen", "Passat", 2024),
            CreateVehicle("BMW 3er", VehicleCategory.Mittelklasse, LocationCode.Of("MUC-FLG"),
                Money.Euro(89.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "BMW", "320d", 2024),
            CreateVehicle("Mercedes C-Klasse", VehicleCategory.Mittelklasse, LocationCode.Of("FRA-FLG"),
                Money.Euro(92.99m), 5, FuelType.Hybrid, TransmissionType.Automatic,
                "Mercedes-Benz", "C 300e", 2024),
            CreateVehicle("Audi A4", VehicleCategory.Mittelklasse, LocationCode.Of("HAM-HBF"),
                Money.Euro(87.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "Audi", "A4 40 TDI", 2024),
            CreateVehicle("Tesla Model 3", VehicleCategory.Mittelklasse, LocationCode.Of("CGN-HBF"),
                Money.Euro(94.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "Tesla", "Model 3", 2024)
        ]);

        // Oberklasse (Upper Class) - 95-150 EUR/day
        vehicles.AddRange([
            CreateVehicle("BMW 5er", VehicleCategory.Oberklasse, LocationCode.Of("BER-HBF"),
                Money.Euro(119.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "BMW", "530d", 2024),
            CreateVehicle("Mercedes E-Klasse", VehicleCategory.Oberklasse, LocationCode.Of("MUC-FLG"),
                Money.Euro(129.99m), 5, FuelType.Hybrid, TransmissionType.Automatic,
                "Mercedes-Benz", "E 300e", 2024),
            CreateVehicle("Audi A6", VehicleCategory.Oberklasse, LocationCode.Of("FRA-FLG"),
                Money.Euro(124.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "Audi", "A6 50 TDI", 2024),
            CreateVehicle("BMW i4", VehicleCategory.Oberklasse, LocationCode.Of("HAM-HBF"),
                Money.Euro(139.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "BMW", "i4 eDrive40", 2024)
        ]);

        // SUVs - 75-135 EUR/day
        vehicles.AddRange([
            CreateVehicle("VW Tiguan", VehicleCategory.SUV, LocationCode.Of("BER-HBF"),
                Money.Euro(79.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "Volkswagen", "Tiguan", 2024),
            CreateVehicle("BMW X3", VehicleCategory.SUV, LocationCode.Of("MUC-FLG"),
                Money.Euro(109.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "BMW", "X3 xDrive30d", 2024),
            CreateVehicle("Mercedes GLC", VehicleCategory.SUV, LocationCode.Of("FRA-FLG"),
                Money.Euro(114.99m), 5, FuelType.Hybrid, TransmissionType.Automatic,
                "Mercedes-Benz", "GLC 300e", 2024),
            CreateVehicle("Audi Q5", VehicleCategory.SUV, LocationCode.Of("HAM-HBF"),
                Money.Euro(112.99m), 5, FuelType.Diesel, TransmissionType.Automatic,
                "Audi", "Q5 45 TDI", 2024),
            CreateVehicle("Tesla Model Y", VehicleCategory.SUV, LocationCode.Of("CGN-HBF"),
                Money.Euro(129.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "Tesla", "Model Y", 2024),
            CreateVehicle("Volvo XC60", VehicleCategory.SUV, LocationCode.Of("BER-HBF"),
                Money.Euro(119.99m), 5, FuelType.Hybrid, TransmissionType.Automatic,
                "Volvo", "XC60 Recharge", 2024),
            CreateVehicle("Mazda CX-5", VehicleCategory.SUV, LocationCode.Of("MUC-FLG"),
                Money.Euro(84.99m), 5, FuelType.Petrol, TransmissionType.Automatic,
                "Mazda", "CX-5", 2023),
            CreateVehicle("Kia Sportage", VehicleCategory.SUV, LocationCode.Of("FRA-FLG"),
                Money.Euro(77.99m), 5, FuelType.Hybrid, TransmissionType.Automatic,
                "Kia", "Sportage", 2024)
        ]);

        // Additional vehicles with varied statuses
        vehicles.AddRange([
            CreateVehicle("VW ID.3", VehicleCategory.Kompaktklasse, LocationCode.Of("BER-HBF"),
                Money.Euro(54.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "Volkswagen", "ID.3", 2024),
            CreateVehicle("VW ID.4", VehicleCategory.SUV, LocationCode.Of("MUC-FLG"),
                Money.Euro(89.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "Volkswagen", "ID.4", 2024),
            CreateVehicle("Hyundai Ioniq 5", VehicleCategory.SUV, LocationCode.Of("FRA-FLG"),
                Money.Euro(94.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "Hyundai", "Ioniq 5", 2024),
            CreateVehicle("Polestar 2", VehicleCategory.Mittelklasse, LocationCode.Of("HAM-HBF"),
                Money.Euro(99.99m), 5, FuelType.Electric, TransmissionType.Automatic,
                "Polestar", "2", 2024)
        ]);

        // Set some vehicles to different statuses for realism
        vehicles[5].MarkAsRented(); // One Kompakt is rented
        vehicles[12].MarkAsRented(); // One Mittel is rented
        vehicles[18].MarkAsUnderMaintenance(); // One SUV is under maintenance

        return vehicles;
    }

    private Vehicle CreateVehicle(
        string name,
        VehicleCategory category,
        LocationCode locationCode,
        Money dailyRate,
        int seats,
        FuelType fuelType,
        TransmissionType transmissionType,
        string? manufacturer = null,
        string? model = null,
        int? year = null)
    {
        var vehicle = Vehicle.From(
            VehicleName.Of(name),
            category,
            locationCode,
            dailyRate,
            SeatingCapacity.Of(seats),
            fuelType,
            transmissionType
        );

        // Set additional details
        if (manufacturer != null || model != null || year != null)
        {
            vehicle.SetDetails(
                manufacturer != null ? Manufacturer.Of(manufacturer) : null,
                model != null ? VehicleModel.Of(model) : null,
                year != null ? ManufacturingYear.Of(year.Value) : null,
                null // No image URLs for now
            );
        }

        // Generate realistic German license plates
        vehicle.SetLicensePlate(GenerateLicensePlate(locationCode));

        return vehicle;
    }

    private string GenerateLicensePlate(LocationCode locationCode)
    {
        // German license plate format: B-XX 1234 (City code - letters - numbers)
        var cityCode = locationCode.Value switch
        {
            "BER-HBF" => "B", // Berlin
            "MUC-FLG" => "M", // München
            "FRA-FLG" => "F", // Frankfurt
            "HAM-HBF" => "HH", // Hamburg
            "CGN-HBF" => "K", // Köln
            _ => "B"
        };

        var letters = $"{(char)Random.Shared.Next('A', 'Z' + 1)}{(char)Random.Shared.Next('A', 'Z' + 1)}";
        var numbers = Random.Shared.Next(1000, 9999);

        return $"{cityCode}-{letters} {numbers}";
    }
}
