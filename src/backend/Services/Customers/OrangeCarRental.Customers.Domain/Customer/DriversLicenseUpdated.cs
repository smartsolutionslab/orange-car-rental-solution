using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

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
