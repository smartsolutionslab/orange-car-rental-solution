using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Repository interface for Customer aggregate.
///     Provides data access operations following the repository pattern.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    ///     Gets a customer by their unique identifier.
    /// </summary>
    /// <param name="id">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when the customer is not found.</exception>
    Task<Customer> GetByIdAsync(
        CustomerIdentifier id,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a customer by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when the customer is not found.</exception>
    Task<Customer> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a customer exists with the given email address.
    ///     Useful for validating uniqueness during registration.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a customer exists with this email, otherwise false.</returns>
    Task<bool> ExistsWithEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a customer exists with the given email address, excluding a specific customer.
    ///     Useful for validating email uniqueness when updating a customer's email.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludeCustomerIdentifier">The customer ID to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if another customer exists with this email, otherwise false.</returns>
    Task<bool> ExistsWithEmailAsync(
        Email email,
        CustomerIdentifier excludeCustomerIdentifier,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all customers (use with caution - prefer SearchAsync for large datasets).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An immutable read-only list of all customers.</returns>
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Searches customers with database-level filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Search parameters including filters and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged result containing matching customers.</returns>
    Task<PagedResult<Customer>> SearchAsync(
        CustomerSearchParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new customer to the repository.
    /// </summary>
    /// <param name="customer">The customer to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing customer in the repository.
    /// </summary>
    /// <param name="customer">The customer to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(
        Customer customer,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a customer (soft delete recommended for GDPR compliance).
    ///     For GDPR "right to be forgotten", anonymize data instead of hard delete.
    /// </summary>
    /// <param name="id">The customer ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(
        CustomerIdentifier id,
        CancellationToken cancellationToken = default);
}
