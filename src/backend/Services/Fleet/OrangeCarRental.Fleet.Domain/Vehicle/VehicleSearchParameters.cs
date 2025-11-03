using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Parameters for searching vehicles in the repository.
/// </summary>
public sealed class VehicleSearchParameters : SearchParameters
{
    /// <summary>
    /// Filter by location (value object).
    /// </summary>
    public LocationCode? LocationCode { get; init; }

    /// <summary>
    /// Filter by vehicle category (value object).
    /// </summary>
    public VehicleCategory? Category { get; init; }

    /// <summary>
    /// Filter by minimum seating capacity.
    /// </summary>
    public int? MinSeats { get; init; }

    /// <summary>
    /// Filter by fuel type.
    /// </summary>
    public FuelType? FuelType { get; init; }

    /// <summary>
    /// Filter by transmission type.
    /// </summary>
    public TransmissionType? TransmissionType { get; init; }

    /// <summary>
    /// Filter by maximum daily rate (gross amount).
    /// </summary>
    public decimal? MaxDailyRateGross { get; init; }

    /// <summary>
    /// Filter by vehicle status.
    /// </summary>
    public VehicleStatus? Status { get; init; }

    /// <summary>
    /// Filter by pickup date for availability.
    /// </summary>
    public DateTime? PickupDate { get; init; }

    /// <summary>
    /// Filter by return date for availability.
    /// </summary>
    public DateTime? ReturnDate { get; init; }
}
