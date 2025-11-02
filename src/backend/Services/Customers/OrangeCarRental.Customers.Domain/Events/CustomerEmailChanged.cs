using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Events;

/// <summary>
/// Domain event raised when a customer's email address is changed.
/// This could trigger email verification workflows, notification updates, etc.
/// </summary>
public sealed record CustomerEmailChanged(
    CustomerId CustomerId,
    Email OldEmail,
    Email NewEmail,
    DateTime ChangedAtUtc
) : DomainEvent;
