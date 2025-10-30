using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;

/// <summary>
/// Parameters for searching vehicles in the repository.
/// </summary>
public sealed class VehicleSearchParameters
{
    public string? LocationCode { get; init; }
    public string? CategoryCode { get; init; }
    public int? MinSeats { get; init; }
    public FuelType? FuelType { get; init; }
    public TransmissionType? TransmissionType { get; init; }
    public decimal? MaxDailyRateGross { get; init; }
    public VehicleStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
