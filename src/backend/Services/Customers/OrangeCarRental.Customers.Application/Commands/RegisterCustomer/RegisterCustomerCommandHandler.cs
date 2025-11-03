using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
/// Handler for RegisterCustomerCommand.
/// Validates email uniqueness, creates a new customer aggregate, and persists to the repository.
/// </summary>
public sealed class RegisterCustomerCommandHandler(ICustomerRepository customers)
{
    /// <summary>
    /// Handles the customer registration command.
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
        // Create value objects from command data
        var email = Email.Of(command.Email);
        var phoneNumber = PhoneNumber.Of(command.PhoneNumber);
        var address = Address.Of(
            command.Street,
            command.City,
            command.PostalCode,
            command.Country);
        var driversLicense = DriversLicense.Of(
            command.LicenseNumber,
            command.LicenseIssueCountry,
            command.LicenseIssueDate,
            command.LicenseExpiryDate);

        // Check email uniqueness
        var emailExists = await customers.ExistsWithEmailAsync(email, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException(
                $"A customer with email '{command.Email}' already exists.");
        }

        // Register new customer (domain method handles all business rules)
        var customer = Customer.Register(
            command.FirstName,
            command.LastName,
            email,
            phoneNumber,
            command.DateOfBirth,
            address,
            driversLicense);

        // Persist to repository
        await customers.AddAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        // Return result
        return new RegisterCustomerResult
        {
            CustomerId = customer.Id.Value,
            Email = customer.Email.Value,
            Status = "Customer registered successfully",
            RegisteredAtUtc = customer.RegisteredAtUtc
        };
    }
}
