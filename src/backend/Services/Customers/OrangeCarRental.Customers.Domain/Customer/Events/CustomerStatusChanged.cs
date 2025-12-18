using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Domain event raised when a customer's account status changes.
///     This could trigger notifications, permission updates, or audit logs.
/// </summary>
public sealed record CustomerStatusChanged(
    CustomerIdentifier CustomerIdentifier,
    CustomerStatus OldStatus,
    CustomerStatus NewStatus,
    string Reason,
    DateTime ChangedAtUtc
) : DomainEvent;
