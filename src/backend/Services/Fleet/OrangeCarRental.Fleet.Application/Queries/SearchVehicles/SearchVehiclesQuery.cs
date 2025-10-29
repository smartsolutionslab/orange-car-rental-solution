namespace OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Query to search available vehicles based on filters.
/// Supports filtering by date range, location, category, and other criteria.
/// </summary>
public sealed record SearchVehiclesQuery
{
    /// <summary>
    /// Pickup date (optional). If provided, only available vehicles on this date are returned.
    /// </summary>
    public DateTime? PickupDate { get; init; }

    /// <summary>
    /// Return date (optional). If provided, vehicles must be available for the entire period.
    /// </summary>
    public DateTime? ReturnDate { get; init; }

    /// <summary>
    /// Location code (e.g., "BER-HBF", "MUC-FLG"). If provided, filter by location.
    /// </summary>
    public string? LocationCode { get; init; }

    /// <summary>
    /// Vehicle category code (e.g., "KLEIN", "MITTEL", "SUV"). If provided, filter by category.
    /// </summary>
    public string? CategoryCode { get; init; }

    /// <summary>
    /// Minimum number of seats required.
    /// </summary>
    public int? MinSeats { get; init; }

    /// <summary>
    /// Fuel type filter (Petrol, Diesel, Electric, Hybrid, etc.)
    /// </summary>
    public string? FuelType { get; init; }

    /// <summary>
    /// Transmission type filter (Manual, Automatic)
    /// </summary>
    public string? TransmissionType { get; init; }

    /// <summary>
    /// Maximum daily rate (gross, in EUR). If provided, filter by price.
    /// </summary>
    public decimal? MaxDailyRateGross { get; init; }

    /// <summary>
    /// Page number for pagination (1-based). Defaults to 1.
    /// </summary>
    public int? PageNumber { get; init; }

    /// <summary>
    /// Page size for pagination. Defaults to 20.
    /// </summary>
    public int? PageSize { get; init; }
}
