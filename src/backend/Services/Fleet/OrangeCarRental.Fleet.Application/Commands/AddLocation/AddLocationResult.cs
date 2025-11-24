namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddLocation;

/// <summary>
///     Result of adding a new location.
/// </summary>
public sealed record AddLocationResult(
    Guid LocationId,
    string Code,
    string Name,
    string FullAddress,
    string Status
);
