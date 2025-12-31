using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands;

/// <summary>
///     Command to register a new customer.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record RegisterCustomerCommand(
    CustomerName Name,
    Email Email,
    PhoneNumber PhoneNumber,
    BirthDate DateOfBirth,
    Address Address,
    DriversLicense DriversLicense) : ICommand<RegisterCustomerResult>;
