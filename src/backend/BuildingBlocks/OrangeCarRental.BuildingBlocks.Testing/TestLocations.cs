namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
/// Centralized location codes for testing.
/// Use these constants instead of hardcoding location codes in tests.
/// </summary>
public static class TestLocations
{
    // German Train Stations (HBF = Hauptbahnhof)
    public const string BerlinHbf = "BER-HBF";
    public const string HamburgHbf = "HAM-HBF";
    public const string MunichHbf = "MUC-HBF";
    public const string FrankfurtHbf = "FRA-HBF";
    public const string CologneHbf = "CGN-HBF";

    // German Airports (FLG = Flughafen)
    public const string MunichAirport = "MUC-FLG";
    public const string FrankfurtAirport = "FRA-FLG";
    public const string BerlinAirport = "BER-FLG";
    public const string DusseldorfAirport = "DUS-FLG";
    public const string HamburgAirport = "HAM-FLG";

    // Default pickup/return locations for tests
    public const string DefaultPickup = BerlinHbf;
    public const string DefaultReturn = BerlinHbf;

    /// <summary>
    /// All available location codes for iteration in tests.
    /// </summary>
    public static readonly string[] All =
    [
        BerlinHbf, HamburgHbf, MunichHbf, FrankfurtHbf, CologneHbf,
        MunichAirport, FrankfurtAirport, BerlinAirport, DusseldorfAirport, HamburgAirport
    ];

    /// <summary>
    /// All train station codes.
    /// </summary>
    public static readonly string[] TrainStations =
    [
        BerlinHbf, HamburgHbf, MunichHbf, FrankfurtHbf, CologneHbf
    ];

    /// <summary>
    /// All airport codes.
    /// </summary>
    public static readonly string[] Airports =
    [
        MunichAirport, FrankfurtAirport, BerlinAirport, DusseldorfAirport, HamburgAirport
    ];
}
