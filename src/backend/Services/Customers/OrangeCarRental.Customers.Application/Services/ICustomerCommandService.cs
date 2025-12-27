using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;

/// <summary>
/// Service for executing commands on Customer aggregates using event sourcing.
/// Provides methods to load, mutate, and persist customer aggregates through the event store.
/// </summary>
public interface ICustomerCommandService
{
    /// <summary>
    /// Registers a new customer.
    /// Creates a new aggregate, applies the Register command, and saves to the event store.
    /// </summary>
    Task<Customer> RegisterAsync(
        CustomerName name,
        Email email,
        PhoneNumber phoneNumber,
        BirthDate dateOfBirth,
        Address address,
        DriversLicense driversLicense,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer's profile.
    /// Loads the aggregate from the event store, applies the update, and saves new events.
    /// </summary>
    Task<Customer> UpdateProfileAsync(
        CustomerIdentifier customerId,
        CustomerName name,
        PhoneNumber phoneNumber,
        Address address,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer's driver's license.
    /// </summary>
    Task<Customer> UpdateDriversLicenseAsync(
        CustomerIdentifier customerId,
        DriversLicense newLicense,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the customer's account status.
    /// </summary>
    Task<Customer> ChangeStatusAsync(
        CustomerIdentifier customerId,
        CustomerStatus newStatus,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the customer's email address.
    /// </summary>
    Task<Customer> UpdateEmailAsync(
        CustomerIdentifier customerId,
        Email newEmail,
        CancellationToken cancellationToken = default);
}
