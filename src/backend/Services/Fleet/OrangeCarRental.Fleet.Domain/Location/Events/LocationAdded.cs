using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location.Events;

/// <summary>
///     Domain event raised when a new location is added.
/// </summary>
public sealed record LocationAdded(
    LocationCode Code,
    LocationName Name
) : DomainEvent;
