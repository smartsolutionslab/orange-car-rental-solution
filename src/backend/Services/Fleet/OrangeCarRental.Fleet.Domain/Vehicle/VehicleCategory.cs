using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Vehicle category (e.g., Kleinwagen, Mittelklasse, Oberklasse, SUV, Transporter).
///     German car rental standard categories.
/// </summary>
public readonly record struct VehicleCategory : IValueObject
{
    // Standard German car rental categories
    public static readonly VehicleCategory Kleinwagen = new("KLEIN", "Kleinwagen");
    public static readonly VehicleCategory Kompaktklasse = new("KOMPAKT", "Kompaktklasse");
    public static readonly VehicleCategory Mittelklasse = new("MITTEL", "Mittelklasse");
    public static readonly VehicleCategory Oberklasse = new("OBER", "Oberklasse");
    public static readonly VehicleCategory SUV = new("SUV", "SUV");
    public static readonly VehicleCategory Kombi = new("KOMBI", "Kombi");
    public static readonly VehicleCategory Transporter = new("TRANS", "Transporter");
    public static readonly VehicleCategory Luxus = new("LUXUS", "Luxusklasse");

    private static readonly Dictionary<string, VehicleCategory> categories = new()
    {
        { Kleinwagen.Code, Kleinwagen },
        { Kompaktklasse.Code, Kompaktklasse },
        { Mittelklasse.Code, Mittelklasse },
        { Oberklasse.Code, Oberklasse },
        { SUV.Code, SUV },
        { Kombi.Code, Kombi },
        { Transporter.Code, Transporter },
        { Luxus.Code, Luxus }
    };

    private VehicleCategory(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; }
    public string Name { get; }

    public static IReadOnlyCollection<VehicleCategory> All => categories.Values.ToList();

    public static VehicleCategory FromCode(string code)
    {
        Ensure.That(code, nameof(code))
            .IsNotNullOrWhiteSpace();

        var upperCode = code.ToUpperInvariant();
        if (!categories.TryGetValue(upperCode, out var category))
            throw new ArgumentException($"Unknown vehicle category code: {code}", nameof(code));

        return category;
    }

    public static VehicleCategory? TryParse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return FromCode(value);
    }

    public override string ToString() => $"{Name} ({Code})";
}
