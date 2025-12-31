using SmartSolutionsLab.OrangeCarRental.Fleet.Api.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Requests;

/// <summary>
///     Request DTO for adding a new vehicle to the fleet.
///     Accepts primitives from HTTP requests and maps to AddVehicleToFleetCommand with value objects.
/// </summary>
public sealed record AddVehicleToFleetRequest(
    VehicleBasicInfoDto BasicInfo,
    VehicleSpecificationsDto Specifications,
    VehicleLocationAndPricingDto LocationAndPricing,
    VehicleRegistrationDto? Registration);
