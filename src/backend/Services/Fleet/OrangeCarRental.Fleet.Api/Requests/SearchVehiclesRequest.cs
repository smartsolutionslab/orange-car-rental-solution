namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Requests;

/// <summary>
///     Request DTO for searching vehicles with filtering and pagination.
///     Accepts primitives from query string and maps to SearchVehiclesQuery with value objects.
/// </summary>
public sealed record SearchVehiclesRequest(
    DateOnly? PickupDate,
    DateOnly? ReturnDate,
    string? LocationCode,
    string? CategoryCode,
    int? MinSeats,
    string? FuelType,
    string? TransmissionType,
    string? Transmission, // Alias for TransmissionType for user-friendliness
    decimal? MaxDailyRateGross,
    string? Status, // Vehicle status filter (Available, Rented, Maintenance, etc.)
    int? PageNumber = null,
    int? PageSize = null)
{
    /// <summary>
    ///     Gets the effective transmission type, preferring TransmissionType over the Transmission alias.
    /// </summary>
    public string? EffectiveTransmissionType => TransmissionType ?? Transmission;
}
