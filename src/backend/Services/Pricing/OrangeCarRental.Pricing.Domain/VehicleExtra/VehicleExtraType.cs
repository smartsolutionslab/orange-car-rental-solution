namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.VehicleExtra;

/// <summary>
///     Types of vehicle extras available for German car rentals.
/// </summary>
public enum VehicleExtraType
{
    /// <summary>
    ///     GPS navigation system (Navigationssystem).
    /// </summary>
    GPS = 1,

    /// <summary>
    ///     Child seat for infants 0-12 months (Babyschale).
    /// </summary>
    ChildSeatInfant = 2,

    /// <summary>
    ///     Child seat for toddlers 1-4 years (Kindersitz).
    /// </summary>
    ChildSeatToddler = 3,

    /// <summary>
    ///     Booster seat for children 4-12 years (Sitzerhöhung).
    /// </summary>
    BoosterSeat = 4,

    /// <summary>
    ///     Winter tires (Winterreifen) - mandatory Nov-Mar in certain conditions.
    /// </summary>
    WinterTires = 5,

    /// <summary>
    ///     Snow chains (Schneeketten).
    /// </summary>
    SnowChains = 6,

    /// <summary>
    ///     Roof rack (Dachgepäckträger).
    /// </summary>
    RoofRack = 7,

    /// <summary>
    ///     Ski rack (Skiträger).
    /// </summary>
    SkiRack = 8,

    /// <summary>
    ///     Bike rack (Fahrradträger).
    /// </summary>
    BikeRack = 9,

    /// <summary>
    ///     Trailer hitch (Anhängerkupplung).
    /// </summary>
    TrailerHitch = 10,

    /// <summary>
    ///     Additional driver (Zusatzfahrer).
    /// </summary>
    AdditionalDriver = 11,

    /// <summary>
    ///     Cross-border travel permit (Grenzübertrittserlaubnis).
    /// </summary>
    CrossBorderPermit = 12
}
