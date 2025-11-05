using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Domain event raised when a new customer registers in the system.
///     This event can trigger welcome emails, analytics tracking, etc.
/// </summary>
public sealed record CustomerRegistered(
    CustomerIdentifier CustomerIdentifier,
    string FirstName,
    string LastName,
    Email Email,
    DateOnly DateOfBirth,
    DateTime RegisteredAtUtc
) : DomainEvent;
