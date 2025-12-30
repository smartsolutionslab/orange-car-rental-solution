namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Result of adding a new location.
/// </summary>
public sealed record AddLocationResult(
    string Code,
    string Name,
    string FullAddress,
    string Status
);
