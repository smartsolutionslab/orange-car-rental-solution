using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Domain event raised when a customer's profile information is updated.
///     Includes name, phone number, and address changes.
/// </summary>
public sealed record CustomerProfileUpdated(
    CustomerId CustomerId,
    string FirstName,
    string LastName,
    PhoneNumber PhoneNumber,
    Address Address,
    DateTime UpdatedAtUtc
) : DomainEvent;
