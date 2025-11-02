using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Events;

/// <summary>
/// Domain event raised when a new customer registers in the system.
/// This event can trigger welcome emails, analytics tracking, etc.
/// </summary>
public sealed record CustomerRegistered(
    CustomerId CustomerId,
    string FirstName,
    string LastName,
    Email Email,
    DateOnly DateOfBirth,
    DateTime RegisteredAtUtc
) : DomainEvent;
