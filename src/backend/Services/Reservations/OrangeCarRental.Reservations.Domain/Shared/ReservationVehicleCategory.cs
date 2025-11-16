namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a vehicle category within the Reservations bounded context.
///     Maps to categories from the Fleet service but is defined locally for context autonomy.
/// </summary>
public readonly record struct ReservationVehicleCategory
{
    private ReservationVehicleCategory(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Category code cannot be empty", nameof(code));

        if (code.Length > 20)
            throw new ArgumentException("Category code cannot exceed 20 characters", nameof(code));

        Code = code.ToUpperInvariant();
    }

    /// <summary>
    ///     The category code (e.g., "KLEIN", "SUV", "LUXUS").
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     Creates a vehicle category from a code string.
    /// </summary>
    /// <param name="code">The category code.</param>
    /// <returns>A new ReservationVehicleCategory instance.</returns>
    public static ReservationVehicleCategory From(string code) => new(code);

    // Predefined categories (matching Fleet service but owned by Reservations context)

    /// <summary>Kleinwagen (Small car)</summary>
    public static readonly ReservationVehicleCategory Kleinwagen = From("KLEIN");

    /// <summary>Kompaktklasse (Compact)</summary>
    public static readonly ReservationVehicleCategory Kompaktklasse = From("KOMPAKT");

    /// <summary>Mittelklasse (Mid-size)</summary>
    public static readonly ReservationVehicleCategory Mittelklasse = From("MITTEL");

    /// <summary>Oberklasse (Premium)</summary>
    public static readonly ReservationVehicleCategory Oberklasse = From("OBER");

    /// <summary>SUV</summary>
    public static readonly ReservationVehicleCategory SUV = From("SUV");

    /// <summary>Kombi (Estate)</summary>
    public static readonly ReservationVehicleCategory Kombi = From("KOMBI");

    /// <summary>Transporter (Van)</summary>
    public static readonly ReservationVehicleCategory Transporter = From("TRANS");

    /// <summary>Luxus (Luxury)</summary>
    public static readonly ReservationVehicleCategory Luxus = From("LUXUS");

    /// <summary>
    ///     Implicit conversion to string for database mapping and serialization.
    /// </summary>
    public static implicit operator string(ReservationVehicleCategory category) => category.Code;

    /// <summary>
    ///     Returns the category code.
    /// </summary>
    public override string ToString() => Code;
}
