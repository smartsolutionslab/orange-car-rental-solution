using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Domain event raised when a new customer registers in the system.
///     This event can trigger welcome emails, analytics tracking, etc.
/// </summary>
public sealed record CustomerRegistered(
    CustomerIdentifier CustomerIdentifier,
    CustomerName Name,
    Email Email,
    BirthDate DateOfBirth,
    DateTime RegisteredAtUtc
) : DomainEvent;
