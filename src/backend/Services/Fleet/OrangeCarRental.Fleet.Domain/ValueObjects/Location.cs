namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Rental station location value object.
/// Represents a German city/station where vehicles can be picked up or returned.
/// </summary>
public readonly record struct Location
{
    public string Code { get; }
    public string City { get; }
    public string Address { get; }
    public string PostalCode { get; }

    private Location(string code, string city, string address, string postalCode)
    {
        Code = code;
        City = city;
        Address = address;
        PostalCode = postalCode;
    }

    public static Location Of(string code, string city, string address = "", string postalCode = "")
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Location code cannot be empty", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty", nameof(city));
        }

        return new Location(
            code.ToUpperInvariant().Trim(),
            city.Trim(),
            address?.Trim() ?? string.Empty,
            postalCode?.Trim() ?? string.Empty
        );
    }

    // Common German rental locations
    public static readonly Location BerlinHauptbahnhof = new(
        "BER-HBF",
        "Berlin",
        "Europaplatz 1",
        "10557"
    );

    public static readonly Location MunichFlughafen = new(
        "MUC-FLG",
        "München",
        "Flughafen München",
        "85356"
    );

    public static readonly Location FrankfurtFlughafen = new(
        "FRA-FLG",
        "Frankfurt",
        "Flughafen Frankfurt",
        "60547"
    );

    public static readonly Location HamburgHauptbahnhof = new(
        "HAM-HBF",
        "Hamburg",
        "Hachmannplatz 16",
        "20099"
    );

    public static readonly Location KolnHauptbahnhof = new(
        "CGN-HBF",
        "Köln",
        "Trankgasse 11",
        "50667"
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

    public override string ToString() => $"{City} - {Code}";
}
