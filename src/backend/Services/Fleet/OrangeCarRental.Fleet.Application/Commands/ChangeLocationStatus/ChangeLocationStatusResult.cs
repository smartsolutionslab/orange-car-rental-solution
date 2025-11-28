namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.ChangeLocationStatus;

/// <summary>
///     Result of changing location status.
/// </summary>
public sealed record ChangeLocationStatusResult(
    string Code,
    string OldStatus,
    string NewStatus,
    string Message
);
