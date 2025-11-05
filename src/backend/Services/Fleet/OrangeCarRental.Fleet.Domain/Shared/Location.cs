namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
///     Rental station location value object.
///     Represents a German city/station where vehicles can be picked up or returned.
/// </summary>
public readonly record struct Location
{
    // Common German rental locations
    public static readonly Location BerlinHauptbahnhof = Of(
        LocationCode.Of("BER-HBF"),
        LocationName.Of("Berlin Hauptbahnhof"),
        Address.Of(Street.Of("Europaplatz 1"), City.Of("Berlin"), PostalCode.Of("10557"))
    );

    public static readonly Location MunichFlughafen = Of(
        LocationCode.Of("MUC-FLG"),
        LocationName.Of("München Flughafen"),
        Address.Of(Street.Of("Flughafen München"), City.Of("München"), PostalCode.Of("85356"))
    );

    public static readonly Location FrankfurtFlughafen = Of(
        LocationCode.Of("FRA-FLG"),
        LocationName.Of("Frankfurt Flughafen"),
        Address.Of(Street.Of("Flughafen Frankfurt"), City.Of("Frankfurt"), PostalCode.Of("60547"))
    );

    public static readonly Location HamburgHauptbahnhof = Of(
        LocationCode.Of("HAM-HBF"),
        LocationName.Of("Hamburg Hauptbahnhof"),
        Address.Of(Street.Of("Hachmannplatz 16"), City.Of("Hamburg"), PostalCode.Of("20099"))
    );

    public static readonly Location KolnHauptbahnhof = Of(
        LocationCode.Of("CGN-HBF"),
        LocationName.Of("Köln Hauptbahnhof"),
        Address.Of(Street.Of("Trankgasse 11"), City.Of("Köln"), PostalCode.Of("50667"))
    );

    private static readonly Dictionary<LocationCode, Location> locations = new()
    {
        { LocationCode.Of("BER-HBF"), BerlinHauptbahnhof },
        { LocationCode.Of("MUC-FLG"), MunichFlughafen },
        { LocationCode.Of("FRA-FLG"), FrankfurtFlughafen },
        { LocationCode.Of("HAM-HBF"), HamburgHauptbahnhof },
        { LocationCode.Of("CGN-HBF"), KolnHauptbahnhof }
    };

    private Location(LocationCode code, LocationName name, Address address)
    {
        Code = code;
        Name = name;
        Address = address;
    }

    public LocationCode Code { get; }
    public LocationName Name { get; }
    public Address Address { get; }

    public static IReadOnlyCollection<Location> All => locations.Values.ToList();

    public static Location Of(LocationCode code, LocationName name, Address address) => new(code, name, address);

    /// <summary>
    ///     Convenience method for backward compatibility
    /// </summary>
    public static Location Of(string code, string city, string address = "", string postalCode = "")
    {
        // For backward compatibility, use city as both name and city in address
        return new Location(
            LocationCode.Of(code),
            LocationName.Of(city),
            Address.Of(address, city, postalCode)
        );
    }

    public static Location FromCode(LocationCode code)
    {
        if (!locations.TryGetValue(code, out var location))
            throw new ArgumentException($"Unknown location code: {code}", nameof(code));

        return location;
    }

    public override string ToString() => $"{Name.Value} ({Code.Value})";
}
