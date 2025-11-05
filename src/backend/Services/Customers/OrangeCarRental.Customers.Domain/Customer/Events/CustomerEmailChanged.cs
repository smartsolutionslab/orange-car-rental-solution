using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Domain event raised when a customer's email address is changed.
///     This could trigger email verification workflows, notification updates, etc.
/// </summary>
public sealed record CustomerEmailChanged(
    CustomerIdentifier CustomerIdentifier,
    Email OldEmail,
    Email NewEmail,
    DateTime ChangedAtUtc
) : DomainEvent;
