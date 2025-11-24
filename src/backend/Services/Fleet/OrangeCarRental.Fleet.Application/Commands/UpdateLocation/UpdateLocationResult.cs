namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateLocation;

/// <summary>
///     Result of updating a location.
/// </summary>
public sealed record UpdateLocationResult(
    Guid LocationId,
    string Code,
    string Name,
    string FullAddress,
    string Message
);
