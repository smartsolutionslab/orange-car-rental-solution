using SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.EventSourcing;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.CommandServices;

/// <summary>
/// Implementation of ICustomerCommandService using event sourcing.
/// Orchestrates loading aggregates from the event store, executing commands, and saving events.
/// </summary>
public sealed class CustomerCommandService : ICustomerCommandService
{
    private readonly IEventSourcedCustomerRepository _repository;

    public CustomerCommandService(IEventSourcedCustomerRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<Customer> RegisterAsync(
        CustomerName name,
        Email email,
        PhoneNumber phoneNumber,
        BirthDate dateOfBirth,
        Address address,
        DriversLicense driversLicense,
        CancellationToken cancellationToken = default)
    {
        var customer = new Customer();
        customer.Register(name, email, phoneNumber, dateOfBirth, address, driversLicense);

        await _repository.SaveAsync(customer, cancellationToken);

        return customer;
    }

    /// <inheritdoc />
    public async Task<Customer> UpdateProfileAsync(
        CustomerIdentifier customerId,
        CustomerName name,
        PhoneNumber phoneNumber,
        Address address,
        CancellationToken cancellationToken = default)
    {
        var customer = await _repository.LoadAsync(customerId, cancellationToken);

        if (!customer.State.HasBeenCreated)
            throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");

        customer.UpdateProfile(name, phoneNumber, address);

        await _repository.SaveAsync(customer, cancellationToken);

        return customer;
    }

    /// <inheritdoc />
    public async Task<Customer> UpdateDriversLicenseAsync(
        CustomerIdentifier customerId,
        DriversLicense newLicense,
        CancellationToken cancellationToken = default)
    {
        var customer = await _repository.LoadAsync(customerId, cancellationToken);

        if (!customer.State.HasBeenCreated)
            throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");

        customer.UpdateDriversLicense(newLicense);

        await _repository.SaveAsync(customer, cancellationToken);

        return customer;
    }

    /// <inheritdoc />
    public async Task<Customer> ChangeStatusAsync(
        CustomerIdentifier customerId,
        CustomerStatus newStatus,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var customer = await _repository.LoadAsync(customerId, cancellationToken);

        if (!customer.State.HasBeenCreated)
            throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");

        customer.ChangeStatus(newStatus, reason);

        await _repository.SaveAsync(customer, cancellationToken);

        return customer;
    }

    /// <inheritdoc />
    public async Task<Customer> UpdateEmailAsync(
        CustomerIdentifier customerId,
        Email newEmail,
        CancellationToken cancellationToken = default)
    {
        var customer = await _repository.LoadAsync(customerId, cancellationToken);

        if (!customer.State.HasBeenCreated)
            throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");

        customer.UpdateEmail(newEmail);

        await _repository.SaveAsync(customer, cancellationToken);

        return customer;
    }
}
