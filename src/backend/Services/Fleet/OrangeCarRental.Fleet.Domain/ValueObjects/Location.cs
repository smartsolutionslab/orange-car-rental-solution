namespace SmartSolutionsLab.Fleet.Domain.ValueObjects;

/// <summary>
/// Rental station location value object.
/// Represents a German city/station where vehicles can be picked up or returned.
/// </summary>
public sealed record Location
{
    public string Code { get; init; }
    public string City { get; init; }
    public string Address { get; init; }
    public string PostalCode { get; init; }

    public Location(string code, string city, string address, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Location code cannot be empty", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty", nameof(city));
        }

        Code = code.ToUpperInvariant().Trim();
        City = city.Trim();
        Address = address?.Trim() ?? string.Empty;
        PostalCode = postalCode?.Trim() ?? string.Empty;
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

    public override string ToString() => $"{City} - {Code}";
}
