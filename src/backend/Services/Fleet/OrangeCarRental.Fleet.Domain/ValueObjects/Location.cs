namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

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

    public static Location Of(string code, string name, string street, string city, string postalCode)
    {
        return new Location(
            LocationCode.Of(code),
            LocationName.Of(name),
            ValueObjects.Address.Of(street, city, postalCode)
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
            ValueObjects.Address.Of(address, city, postalCode)
        );
    }

    // Common German rental locations
    public static readonly Location BerlinHauptbahnhof = Location.Of(
        code: "BER-HBF",
        name: "Berlin Hauptbahnhof",
        street: "Europaplatz 1",
        city: "Berlin",
        postalCode: "10557"
    );

    public static readonly Location MunichFlughafen = Location.Of(
        code: "MUC-FLG",
        name: "München Flughafen",
        street: "Flughafen München",
        city: "München",
        postalCode: "85356"
    );

    public static readonly Location FrankfurtFlughafen = Location.Of(
        code: "FRA-FLG",
        name: "Frankfurt Flughafen",
        street: "Flughafen Frankfurt",
        city: "Frankfurt",
        postalCode: "60547"
    );

    public static readonly Location HamburgHauptbahnhof = Location.Of(
        code: "HAM-HBF",
        name: "Hamburg Hauptbahnhof",
        street: "Hachmannplatz 16",
        city: "Hamburg",
        postalCode: "20099"
    );

    public static readonly Location KolnHauptbahnhof = Location.Of(
        code: "CGN-HBF",
        name: "Köln Hauptbahnhof",
        street: "Trankgasse 11",
        city: "Köln",
        postalCode: "50667"
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
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Location code cannot be empty", nameof(code));
        }

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
