using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Events;

/// <summary>
/// Domain event raised when a customer's account status changes.
/// This could trigger notifications, permission updates, or audit logs.
/// </summary>
public sealed record CustomerStatusChanged(
    CustomerId CustomerId,
    CustomerStatus OldStatus,
    CustomerStatus NewStatus,
    string Reason,
    DateTime ChangedAtUtc
) : DomainEvent;
