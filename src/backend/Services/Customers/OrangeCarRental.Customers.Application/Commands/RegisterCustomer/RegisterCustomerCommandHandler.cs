using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Handler for RegisterCustomerCommand.
///     Validates email uniqueness, creates a new customer aggregate, and persists to the repository.
/// </summary>
public sealed class RegisterCustomerCommandHandler(ICustomerRepository customers)
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
        // Check email uniqueness
        var emailExists = await customers.ExistsWithEmailAsync(command.Email, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException(
                $"A customer with email '{command.Email.Value}' already exists.");
        }

        // Register new customer (domain method handles all business rules)
        var customer = Customer.Register(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth,
            command.Address,
            command.DriversLicense);

        // Persist to repository
        await customers.AddAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        // Return result
        return new RegisterCustomerResult
        {
            CustomerIdentifier = customer.Id.Value,
            Email = customer.Email.Value,
            Status = "Customer registered successfully",
            RegisteredAtUtc = customer.RegisteredAtUtc
        };
    }
}
