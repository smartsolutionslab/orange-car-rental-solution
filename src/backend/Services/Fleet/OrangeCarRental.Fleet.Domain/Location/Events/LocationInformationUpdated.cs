using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location.Events;

/// <summary>
///     Domain event raised when location information is updated.
/// </summary>
public sealed record LocationInformationUpdated(
    LocationIdentifier LocationId,
    LocationName Name,
    Address Address
) : DomainEvent;
