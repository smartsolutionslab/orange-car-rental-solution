namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
/// Centralized vehicle category codes for testing.
/// Based on German car rental market categories.
/// </summary>
public static class TestVehicleCategories
{
    // Standard German rental categories
    public const string Klein = "KLEIN";       // Small (e.g., VW Up)
    public const string Kompakt = "KOMPAKT";   // Compact (e.g., VW Golf)
    public const string Mittel = "MITTEL";     // Mid-size (e.g., VW Passat)
    public const string Ober = "OBER";         // Upper class (e.g., BMW 5er)
    public const string Suv = "SUV";           // SUV (e.g., VW Tiguan)
    public const string Luxus = "LUXUS";       // Luxury (e.g., Mercedes S-Class)
    public const string Kombi = "KOMBI";       // Station wagon (e.g., VW Passat Variant)
    public const string Van = "VAN";           // Van (e.g., VW Touran)

    // Default category for tests
    public const string Default = Kompakt;

    /// <summary>
    /// All available categories.
    /// </summary>
    public static readonly string[] All =
    [
        Klein, Kompakt, Mittel, Ober, Suv, Luxus, Kombi, Van
    ];

    /// <summary>
    /// Standard daily rates (net) per category for test calculations.
    /// </summary>
    public static decimal GetDailyRateNet(string category) => category switch
    {
        Klein => 29.00m,
        Kompakt => 39.00m,
        Mittel => 59.00m,
        Ober => 89.00m,
        Suv => 69.00m,
        Luxus => 149.00m,
        Kombi => 49.00m,
        Van => 59.00m,
        _ => 49.00m
    };
}
