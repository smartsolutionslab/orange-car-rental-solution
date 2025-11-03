namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
/// Rental station location value object.
/// Represents a German city/station where vehicles can be picked up or returned.
/// </summary>
public readonly record struct Location
{
    public LocationCode Code { get; }
    public LocationName Name { get; }
    public Address Address { get; }

    private Location(LocationCode code, LocationName name, Address address)
    {
        Code = code;
        Name = name;
        Address = address;
    }

    public static Location Of(LocationCode code, LocationName name, Address address) => new(code, name, address);

    public static Location Of(string code, string name, string street, string city, string postalCode)
    {
        return new Location(
            LocationCode.Of(code),
            LocationName.Of(name),
            Address.Of(street, city, postalCode)
        );
    }

    /// <summary>
    /// Convenience method for backward compatibility
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

    // Common German rental locations
    public static readonly Location BerlinHauptbahnhof = Location.Of(
        LocationCode.Of("BER-HBF"),
        LocationName.Of("Berlin Hauptbahnhof"),
        Address.Of(Street.Of("Europaplatz 1"), City.Of("Berlin"), PostalCode.Of("10557"))
    );

    public static readonly Location MunichFlughafen = Location.Of(
        LocationCode.Of("MUC-FLG"),
        LocationName.Of("München Flughafen"),
        Address.Of(Street.Of("Flughafen München"), City.Of("München"), PostalCode.Of("85356"))
    );

    public static readonly Location FrankfurtFlughafen = Location.Of(
        LocationCode.Of("FRA-FLG"),
        LocationName.Of("Frankfurt Flughafen"),
        Address.Of(Street.Of("Flughafen Frankfurt"), City.Of("Frankfurt"), PostalCode.Of("60547"))
    );

    public static readonly Location HamburgHauptbahnhof = Location.Of(
        LocationCode.Of("HAM-HBF"),
        LocationName.Of("Hamburg Hauptbahnhof"),
        Address.Of(Street.Of("Hachmannplatz 16"), City.Of("Hamburg"), PostalCode.Of("20099"))
    );

    public static readonly Location KolnHauptbahnhof = Location.Of(
        LocationCode.Of("CGN-HBF"),
        LocationName.Of("Köln Hauptbahnhof"),
        Address.Of(Street.Of("Trankgasse 11"), City.Of("Köln"), PostalCode.Of("50667"))
    );

    private static readonly Dictionary<string, Location> _locations = new()
    {
        { "BER-HBF", BerlinHauptbahnhof },
        { "MUC-FLG", MunichFlughafen },
        { "FRA-FLG", FrankfurtFlughafen },
        { "HAM-HBF", HamburgHauptbahnhof },
        { "CGN-HBF", KolnHauptbahnhof }
    };

    public static Location FromCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        string upperCode = code.ToUpperInvariant();
        if (!_locations.TryGetValue(upperCode, out Location location))
        {
            throw new ArgumentException($"Unknown location code: {code}", nameof(code));
        }

        return location;
    }

    public static IReadOnlyCollection<Location> All => _locations.Values.ToList();

    public override string ToString() => $"{Name.Value} ({Code.Value})";
}
