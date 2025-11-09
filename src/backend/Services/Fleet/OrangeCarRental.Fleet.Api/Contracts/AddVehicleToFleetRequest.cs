namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Contracts;

/// <summary>
///     Request DTO for adding a new vehicle to the fleet.
///     Accepts primitives from HTTP requests and maps to AddVehicleToFleetCommand with value objects.
/// </summary>
public sealed record AddVehicleToFleetRequest
{
    public required VehicleBasicInfoDto BasicInfo { get; init; }
    public required VehicleSpecificationsDto Specifications { get; init; }
    public required VehicleLocationAndPricingDto LocationAndPricing { get; init; }
    public VehicleRegistrationDto? Registration { get; init; }
}

/// <summary>
///     Basic vehicle information.
/// </summary>
public sealed record VehicleBasicInfoDto
{
    /// <summary>Vehicle name (e.g., "VW Golf 8" or "BMW 3er")</summary>
    public required string Name { get; init; }

    /// <summary>Manufacturer name (optional, e.g., "Volkswagen", "BMW")</summary>
    public string? Manufacturer { get; init; }

    /// <summary>Vehicle model (optional, e.g., "Golf", "3 Series")</summary>
    public string? Model { get; init; }

    /// <summary>Manufacturing year (optional, e.g., 2023)</summary>
    public int? Year { get; init; }

    /// <summary>Image URL (optional)</summary>
    public string? ImageUrl { get; init; }
}

/// <summary>
///     Vehicle technical specifications.
/// </summary>
public sealed record VehicleSpecificationsDto
{
    /// <summary>Vehicle category code (e.g., "KOMPAKT", "MITTELKLASSE", "OBERKLASSE")</summary>
    public required string Category { get; init; }

    /// <summary>Number of seats (1-9)</summary>
    public required int Seats { get; init; }

    /// <summary>Fuel type (Petrol, Diesel, Electric, Hybrid)</summary>
    public required string FuelType { get; init; }

    /// <summary>Transmission type (Manual, Automatic)</summary>
    public required string TransmissionType { get; init; }
}

/// <summary>
///     Vehicle location and pricing information.
/// </summary>
public sealed record VehicleLocationAndPricingDto
{
    /// <summary>Current location code (e.g., "BER-HBF", "MUC-FLG")</summary>
    public required string LocationCode { get; init; }

    /// <summary>Daily rental rate (net amount in EUR, 19% VAT will be calculated)</summary>
    public required decimal DailyRateNet { get; init; }
}

/// <summary>
///     Vehicle registration information.
/// </summary>
public sealed record VehicleRegistrationDto
{
    /// <summary>License plate (optional, can be set later)</summary>
    public string? LicensePlate { get; init; }
}
