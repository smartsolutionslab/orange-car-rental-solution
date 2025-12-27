using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a vehicle category within the Reservations bounded context.
///     Maps to categories from the Fleet service but is defined locally for context autonomy.
/// </summary>
public readonly record struct VehicleCategory(string Code) : IValueObject
{
    /// <summary>
    ///     Creates a vehicle category from a code string.
    /// </summary>
    /// <param name="code">The category code.</param>
    /// <returns>A new ReservationVehicleCategory instance.</returns>
    public static VehicleCategory From(string code)
    {
        Ensure.That(code, nameof(code))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(20);

        var value = code.ToUpperInvariant();

        return new VehicleCategory(value);
    }

    public static VehicleCategory? FromNullable(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        return From(code);
    }

    // Predefined categories (matching Fleet service but owned by Reservations context)

    /// <summary>Kleinwagen (Small car)</summary>
    public static readonly VehicleCategory Kleinwagen = From("KLEIN");

    /// <summary>Kompaktklasse (Compact)</summary>
    public static readonly VehicleCategory Kompaktklasse = From("KOMPAKT");

    /// <summary>Mittelklasse (Mid-size)</summary>
    public static readonly VehicleCategory Mittelklasse = From("MITTEL");

    /// <summary>Oberklasse (Premium)</summary>
    public static readonly VehicleCategory Oberklasse = From("OBER");

    /// <summary>SUV</summary>
    public static readonly VehicleCategory SUV = From("SUV");

    /// <summary>Kombi (Estate)</summary>
    public static readonly VehicleCategory Kombi = From("KOMBI");

    /// <summary>Transporter (Van)</summary>
    public static readonly VehicleCategory Transporter = From("TRANS");

    /// <summary>Luxus (Luxury)</summary>
    public static readonly VehicleCategory Luxus = From("LUXUS");

    /// <summary>
    ///     Implicit conversion to string for database mapping and serialization.
    /// </summary>
    public static implicit operator string(VehicleCategory category) => category.Code;

    /// <summary>
    ///     Returns the category code.
    /// </summary>
    public override string ToString() => Code;
}
