using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Handler for RegisterCustomerCommand.
///     Validates email uniqueness, creates a new customer aggregate, and persists to the database.
/// </summary>
public sealed class RegisterCustomerCommandHandler(
    ICustomerRepository repository)
    : ICommandHandler<RegisterCustomerCommand, RegisterCustomerResult>
{
    /// <summary>
    ///     Handles the customer registration command.
    /// </summary>
    /// <param name="command">The registration command with customer data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Registration result with customer ID and details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when email already exists.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public async Task<RegisterCustomerResult> HandleAsync(
        RegisterCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var (customerName, email, phoneNumber, dateOfBirth, address, driversLicense) = command;

        // Check email uniqueness
        var emailExists = await repository.ExistsWithEmailAsync(email, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException($"A customer with email '{email.Value}' already exists.");
        }

        // Create customer using static factory method
        var customer = Customer.Register(customerName, email, phoneNumber, dateOfBirth, address, driversLicense);

        // Persist to database
        await repository.AddAsync(customer, cancellationToken);

        return new RegisterCustomerResult(
            customer.Id.Value,
            customer.Email.Value,
            "Customer registered successfully",
            customer.RegisteredAtUtc);
    }
}
