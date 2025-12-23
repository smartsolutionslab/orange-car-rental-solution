using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Domain event raised when a new customer registers in the system.
///     Contains all data needed to construct the initial customer state.
/// </summary>
public sealed record CustomerRegistered(
    CustomerIdentifier CustomerIdentifier,
    CustomerName Name,
    Email Email,
    PhoneNumber PhoneNumber,
    BirthDate DateOfBirth,
    Address Address,
    DriversLicense DriversLicense,
    DateTime RegisteredAtUtc
) : DomainEvent;
