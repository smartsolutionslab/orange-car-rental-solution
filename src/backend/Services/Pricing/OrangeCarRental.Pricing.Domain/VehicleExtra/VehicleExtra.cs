using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.VehicleExtra;

/// <summary>
///     Vehicle extra (add-on option) for German car rentals.
///     Represents additional equipment or services that can be added to a rental.
/// </summary>
public sealed record VehicleExtra : IValueObject
{
    private VehicleExtra(
        VehicleExtraType type,
        Money dailyRate,
        bool requiresAdvanceBooking,
        bool isPerRental)
    {
        Type = type;
        DailyRate = dailyRate;
        RequiresAdvanceBooking = requiresAdvanceBooking;
        IsPerRental = isPerRental;
    }

    /// <summary>
    ///     Gets the extra type.
    /// </summary>
    public VehicleExtraType Type { get; }

    /// <summary>
    ///     Gets the daily rate for this extra (or one-time fee if IsPerRental is true).
    /// </summary>
    public Money DailyRate { get; }

    /// <summary>
    ///     Gets whether this extra requires advance booking.
    /// </summary>
    public bool RequiresAdvanceBooking { get; }

    /// <summary>
    ///     Gets whether this is a per-rental fee (true) or per-day fee (false).
    /// </summary>
    public bool IsPerRental { get; }

    /// <summary>
    ///     Calculates the total cost for this extra.
    /// </summary>
    /// <param name="rentalDays">Number of rental days.</param>
    /// <returns>Total cost for this extra.</returns>
    public Money CalculateCost(int rentalDays)
    {
        if (IsPerRental)
            return DailyRate;

        return DailyRate * rentalDays;
    }

    /// <summary>
    ///     Gets the German display name for this extra.
    /// </summary>
    public string GetGermanDisplayName() => Type switch
    {
        VehicleExtraType.GPS => "Navigationssystem",
        VehicleExtraType.ChildSeatInfant => "Babyschale (0-12 Monate)",
        VehicleExtraType.ChildSeatToddler => "Kindersitz (1-4 Jahre)",
        VehicleExtraType.BoosterSeat => "Sitzerhöhung (4-12 Jahre)",
        VehicleExtraType.WinterTires => "Winterreifen",
        VehicleExtraType.SnowChains => "Schneeketten",
        VehicleExtraType.RoofRack => "Dachgepäckträger",
        VehicleExtraType.SkiRack => "Skiträger",
        VehicleExtraType.BikeRack => "Fahrradträger",
        VehicleExtraType.TrailerHitch => "Anhängerkupplung",
        VehicleExtraType.AdditionalDriver => "Zusatzfahrer",
        VehicleExtraType.CrossBorderPermit => "Grenzübertrittserlaubnis",
        _ => Type.ToString()
    };

    /// <summary>
    ///     Gets the English display name for this extra.
    /// </summary>
    public string GetEnglishDisplayName() => Type switch
    {
        VehicleExtraType.GPS => "GPS Navigation",
        VehicleExtraType.ChildSeatInfant => "Infant Car Seat (0-12 months)",
        VehicleExtraType.ChildSeatToddler => "Toddler Car Seat (1-4 years)",
        VehicleExtraType.BoosterSeat => "Booster Seat (4-12 years)",
        VehicleExtraType.WinterTires => "Winter Tires",
        VehicleExtraType.SnowChains => "Snow Chains",
        VehicleExtraType.RoofRack => "Roof Rack",
        VehicleExtraType.SkiRack => "Ski Rack",
        VehicleExtraType.BikeRack => "Bike Rack",
        VehicleExtraType.TrailerHitch => "Trailer Hitch",
        VehicleExtraType.AdditionalDriver => "Additional Driver",
        VehicleExtraType.CrossBorderPermit => "Cross-Border Travel Permit",
        _ => Type.ToString()
    };

    public override string ToString() => $"{GetEnglishDisplayName()} ({DailyRate})";

    // Predefined German market extras with typical pricing

    /// <summary>
    ///     GPS Navigation - 5€/day.
    /// </summary>
    public static readonly VehicleExtra GPS = new(
        VehicleExtraType.GPS,
        Money.EuroGross(5.00m),
        requiresAdvanceBooking: false,
        isPerRental: false);

    /// <summary>
    ///     Infant car seat (0-12 months) - 8€/day, requires advance booking.
    /// </summary>
    public static readonly VehicleExtra ChildSeatInfant = new(
        VehicleExtraType.ChildSeatInfant,
        Money.EuroGross(8.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Toddler car seat (1-4 years) - 8€/day, requires advance booking.
    /// </summary>
    public static readonly VehicleExtra ChildSeatToddler = new(
        VehicleExtraType.ChildSeatToddler,
        Money.EuroGross(8.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Booster seat (4-12 years) - 5€/day, requires advance booking.
    /// </summary>
    public static readonly VehicleExtra BoosterSeat = new(
        VehicleExtraType.BoosterSeat,
        Money.EuroGross(5.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Winter tires - included in winter months (Nov-Mar).
    /// </summary>
    public static readonly VehicleExtra WinterTires = new(
        VehicleExtraType.WinterTires,
        Money.Zero(Currency.EUR),
        requiresAdvanceBooking: false,
        isPerRental: true);

    /// <summary>
    ///     Snow chains - 15€ per rental.
    /// </summary>
    public static readonly VehicleExtra SnowChains = new(
        VehicleExtraType.SnowChains,
        Money.EuroGross(15.00m),
        requiresAdvanceBooking: true,
        isPerRental: true);

    /// <summary>
    ///     Roof rack - 10€/day.
    /// </summary>
    public static readonly VehicleExtra RoofRack = new(
        VehicleExtraType.RoofRack,
        Money.EuroGross(10.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Ski rack - 8€/day.
    /// </summary>
    public static readonly VehicleExtra SkiRack = new(
        VehicleExtraType.SkiRack,
        Money.EuroGross(8.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Bike rack - 12€/day.
    /// </summary>
    public static readonly VehicleExtra BikeRack = new(
        VehicleExtraType.BikeRack,
        Money.EuroGross(12.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Trailer hitch - 20€/day.
    /// </summary>
    public static readonly VehicleExtra TrailerHitch = new(
        VehicleExtraType.TrailerHitch,
        Money.EuroGross(20.00m),
        requiresAdvanceBooking: true,
        isPerRental: false);

    /// <summary>
    ///     Additional driver - 10€/day.
    /// </summary>
    public static readonly VehicleExtra AdditionalDriver = new(
        VehicleExtraType.AdditionalDriver,
        Money.EuroGross(10.00m),
        requiresAdvanceBooking: false,
        isPerRental: false);

    /// <summary>
    ///     Cross-border travel permit - 15€ per rental.
    /// </summary>
    public static readonly VehicleExtra CrossBorderPermit = new(
        VehicleExtraType.CrossBorderPermit,
        Money.EuroGross(15.00m),
        requiresAdvanceBooking: false,
        isPerRental: true);

    /// <summary>
    ///     Gets all available extras.
    /// </summary>
    public static IReadOnlyList<VehicleExtra> GetAllExtras() =>
    [
        GPS,
        ChildSeatInfant,
        ChildSeatToddler,
        BoosterSeat,
        WinterTires,
        SnowChains,
        RoofRack,
        SkiRack,
        BikeRack,
        TrailerHitch,
        AdditionalDriver,
        CrossBorderPermit
    ];

    /// <summary>
    ///     Gets an extra by type.
    /// </summary>
    public static VehicleExtra FromType(VehicleExtraType type) => type switch
    {
        VehicleExtraType.GPS => GPS,
        VehicleExtraType.ChildSeatInfant => ChildSeatInfant,
        VehicleExtraType.ChildSeatToddler => ChildSeatToddler,
        VehicleExtraType.BoosterSeat => BoosterSeat,
        VehicleExtraType.WinterTires => WinterTires,
        VehicleExtraType.SnowChains => SnowChains,
        VehicleExtraType.RoofRack => RoofRack,
        VehicleExtraType.SkiRack => SkiRack,
        VehicleExtraType.BikeRack => BikeRack,
        VehicleExtraType.TrailerHitch => TrailerHitch,
        VehicleExtraType.AdditionalDriver => AdditionalDriver,
        VehicleExtraType.CrossBorderPermit => CrossBorderPermit,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown vehicle extra type")
    };
}
