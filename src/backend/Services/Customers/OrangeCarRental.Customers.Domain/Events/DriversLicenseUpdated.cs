using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Events;

/// <summary>
/// Domain event raised when a customer's driver's license information is updated.
/// This could trigger validation checks, expiry notifications, etc.
/// </summary>
public sealed record DriversLicenseUpdated(
    CustomerId CustomerId,
    DriversLicense OldLicense,
    DriversLicense NewLicense,
    DateTime UpdatedAtUtc
) : DomainEvent;
