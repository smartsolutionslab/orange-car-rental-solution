namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

/// <summary>
/// Service for managing customers via the Customers API.
/// </summary>
public interface ICustomersService
{
    /// <summary>
    /// Register a new customer.
    /// </summary>
    /// <param name="request">Customer registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The newly created customer ID</returns>
    Task<Guid> RegisterCustomerAsync(
        RegisterCustomerDto request,
        CancellationToken cancellationToken = default);
}
