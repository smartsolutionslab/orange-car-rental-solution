namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Vehicle category (e.g., Kleinwagen, Mittelklasse, Oberklasse, SUV, Transporter).
/// German car rental standard categories.
/// </summary>
public readonly record struct VehicleCategory
{
    public string Code { get; }
    public string Name { get; }

    private VehicleCategory(string code, string name)
    {
        Code = code;
        Name = name;
    }

    // Standard German car rental categories
    public static readonly VehicleCategory Kleinwagen = new("KLEIN", "Kleinwagen");
    public static readonly VehicleCategory Kompaktklasse = new("KOMPAKT", "Kompaktklasse");
    public static readonly VehicleCategory Mittelklasse = new("MITTEL", "Mittelklasse");
    public static readonly VehicleCategory Oberklasse = new("OBER", "Oberklasse");
    public static readonly VehicleCategory SUV = new("SUV", "SUV");
    public static readonly VehicleCategory Kombi = new("KOMBI", "Kombi");
    public static readonly VehicleCategory Transporter = new("TRANS", "Transporter");
    public static readonly VehicleCategory Luxus = new("LUXUS", "Luxusklasse");

    private static readonly Dictionary<string, VehicleCategory> _categories = new()
    {
        { "KLEIN", Kleinwagen },
        { "KOMPAKT", Kompaktklasse },
        { "MITTEL", Mittelklasse },
        { "OBER", Oberklasse },
        { "SUV", SUV },
        { "KOMBI", Kombi },
        { "TRANS", Transporter },
        { "LUXUS", Luxus }
    };

    public static VehicleCategory FromCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        string upperCode = code.ToUpperInvariant();
        if (!_categories.TryGetValue(upperCode, out VehicleCategory category))
        {
            throw new ArgumentException($"Unknown vehicle category code: {code}", nameof(code));
        }

        return category;
    }

    public static IReadOnlyCollection<VehicleCategory> All => _categories.Values.ToList();

    public override string ToString() => $"{Name} ({Code})";
}
