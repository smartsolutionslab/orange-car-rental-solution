using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Command to register a new customer.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record RegisterCustomerCommand : ICommand<RegisterCustomerResult>
{
    /// <summary>
    ///     Customer's name (includes first name, last name, and optional salutation).
    /// </summary>
    public required CustomerName Name { get; init; }

    /// <summary>
    ///     Customer's email address (value object with validation).
    /// </summary>
    public required Email Email { get; init; }

    /// <summary>
    ///     Customer's phone number (value object with German format validation).
    /// </summary>
    public required PhoneNumber PhoneNumber { get; init; }

    /// <summary>
    ///     Customer's date of birth.
    ///     Must be at least 18 years old.
    /// </summary>
    public required BirthDate DateOfBirth { get; init; }

    /// <summary>
    ///     Customer's address (value object containing street, city, postal code, and country).
    /// </summary>
    public required Address Address { get; init; }

    /// <summary>
    ///     Driver's license (value object with validation).
    /// </summary>
    public required DriversLicense DriversLicense { get; init; }
}
