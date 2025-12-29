namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Constants for vehicle sort field names.
///     Use these instead of magic strings when specifying sort fields.
/// </summary>
public static class VehicleSortFields
{
    // Name and category
    public const string Name = "name";
    public const string Category = "category";
    public const string CategoryCode = "categorycode";

    // Location
    public const string Location = "location";
    public const string LocationCode = "locationcode";

    // Specifications
    public const string Seats = "seats";
    public const string FuelType = "fueltype";
    public const string Fuel = "fuel";
    public const string TransmissionType = "transmissiontype";
    public const string Transmission = "transmission";

    // Pricing
    public const string DailyRate = "dailyrate";
    public const string Price = "price";
    public const string Rate = "rate";

    // Status
    public const string Status = "status";

    /// <summary>
    ///     Default sort field for vehicle queries.
    /// </summary>
    public const string Default = Name;

    /// <summary>
    ///     All valid sort field names.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        Name, Category, CategoryCode,
        Location, LocationCode,
        Seats, FuelType, Fuel, TransmissionType, Transmission,
        DailyRate, Price, Rate,
        Status
    ];
}
