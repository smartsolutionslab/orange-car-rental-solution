namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Result of customer registration operation.
///     Contains the newly created customer's identifier and confirmation details.
/// </summary>
public sealed record RegisterCustomerResult
{
    /// <summary>
    ///     The unique identifier of the newly registered customer.
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    ///     The customer's email address (normalized).
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Registration status message.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    ///     Date and time when the customer was registered (UTC).
    /// </summary>
    public required DateTime RegisteredAtUtc { get; init; }
}
