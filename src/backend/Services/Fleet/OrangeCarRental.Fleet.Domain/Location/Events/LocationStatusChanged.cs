using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location.Events;

/// <summary>
///     Domain event raised when a location's status changes.
/// </summary>
public sealed record LocationStatusChanged(
    LocationIdentifier LocationId,
    LocationCode Code,
    LocationStatus OldStatus,
    LocationStatus NewStatus,
    string? Reason
) : DomainEvent;
