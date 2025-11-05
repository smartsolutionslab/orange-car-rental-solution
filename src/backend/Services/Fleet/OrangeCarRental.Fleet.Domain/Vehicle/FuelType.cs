namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Vehicle fuel type (Kraftstoffart).
/// </summary>
public enum FuelType
{
    /// <summary>Benzin</summary>
    Petrol = 1,

    /// <summary>Diesel</summary>
    Diesel = 2,

    /// <summary>Elektro</summary>
    Electric = 3,

    /// <summary>Hybrid (Benzin + Elektro)</summary>
    Hybrid = 4,

    /// <summary>Plug-in Hybrid</summary>
    PlugInHybrid = 5,

    /// <summary>Wasserstoff</summary>
    Hydrogen = 6
}
