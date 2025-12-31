namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Result of updating a location.
/// </summary>
public sealed record UpdateLocationResult(
    string Code,
    string Name,
    string FullAddress,
    string Message
);
