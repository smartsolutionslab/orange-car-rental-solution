namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Data transfer object for vehicle search results.
/// Contains all information needed to display a vehicle to customers with German pricing.
/// </summary>
public sealed record VehicleDto
{
    /// <summary>
    /// Unique vehicle identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Vehicle name (e.g., "VW Golf", "BMW 3er").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Category code (e.g., "KLEIN", "MITTEL", "SUV").
    /// </summary>
    public required string CategoryCode { get; init; }

    /// <summary>
    /// Category display name (e.g., "Kleinwagen", "Mittelklasse").
    /// </summary>
    public required string CategoryName { get; init; }

    /// <summary>
    /// Current location code (e.g., "BER-HBF").
    /// </summary>
    public required string LocationCode { get; init; }

    /// <summary>
    /// City name (e.g., "Berlin", "München").
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Number of seats.
    /// </summary>
    public required int Seats { get; init; }

    /// <summary>
    /// Fuel type (e.g., "Petrol", "Diesel", "Electric").
    /// </summary>
    public required string FuelType { get; init; }

    /// <summary>
    /// Transmission type (e.g., "Manual", "Automatic").
    /// </summary>
    public required string TransmissionType { get; init; }

    /// <summary>
    /// Daily rental rate - net amount (excluding VAT) in EUR.
    /// </summary>
    public required decimal DailyRateNet { get; init; }

    /// <summary>
    /// Daily rental rate - VAT amount (19%) in EUR.
    /// </summary>
    public required decimal DailyRateVat { get; init; }

    /// <summary>
    /// Daily rental rate - gross amount (including 19% VAT) in EUR.
    /// This is the price the customer pays per day.
    /// </summary>
    public required decimal DailyRateGross { get; init; }

    /// <summary>
    /// Currency code (always "EUR" for German market).
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Vehicle status (e.g., "Available", "Rented").
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// License plate (optional, may be hidden for privacy).
    /// </summary>
    public string? LicensePlate { get; init; }

    /// <summary>
    /// Manufacturer (e.g., "Volkswagen", "BMW").
    /// </summary>
    public string? Manufacturer { get; init; }

    /// <summary>
    /// Model (e.g., "Golf", "3er").
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Manufacturing year.
    /// </summary>
    public int? Year { get; init; }

    /// <summary>
    /// Image URL for vehicle photo.
    /// </summary>
    public string? ImageUrl { get; init; }
}
