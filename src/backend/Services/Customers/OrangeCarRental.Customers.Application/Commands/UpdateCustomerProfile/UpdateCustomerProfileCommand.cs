using System.Diagnostics.CodeAnalysis;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Command to update a customer's profile information.
///     Uses value objects for type safety and early validation.
///     Does not include email or driver's license (use separate commands for those).
/// </summary>
[method: SetsRequiredMembers]
public sealed record UpdateCustomerProfileCommand(
    CustomerIdentifier CustomerId,
    CustomerName Name,
    PhoneNumber PhoneNumber,
    Address Address)
    : ICommand<UpdateCustomerProfileResult>;
