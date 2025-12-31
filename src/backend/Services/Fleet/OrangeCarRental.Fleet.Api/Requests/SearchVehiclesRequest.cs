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
    decimal? MaxDailyRateGross,
    int? PageNumber = null,
    int? PageSize = null);
