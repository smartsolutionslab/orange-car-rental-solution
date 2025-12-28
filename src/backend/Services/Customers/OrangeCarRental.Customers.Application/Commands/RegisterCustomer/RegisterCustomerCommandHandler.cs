using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Handler for RegisterCustomerCommand.
///     Validates email uniqueness, creates a new customer aggregate, and persists via event sourcing.
/// </summary>
public sealed class RegisterCustomerCommandHandler(
    ICustomerRepository repository,
    ICustomersUnitOfWork unitOfWork)
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

        // Check email uniqueness using read model
        var emailExists = await unitOfWork.Customers.ExistsWithEmailAsync(email, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException($"A customer with email '{email.Value}' already exists.");
        }

        // Create customer aggregate and execute domain logic
        var customer = new Customer();
        customer.Register(customerName, email, phoneNumber, dateOfBirth, address, driversLicense);

        // Persist to repository
        await repository.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterCustomerResult(
            customer.Id.Value,
            customer.Email?.Value ?? email.Value,
            "Customer registered successfully",
            customer.RegisteredAtUtc);
    }
}
