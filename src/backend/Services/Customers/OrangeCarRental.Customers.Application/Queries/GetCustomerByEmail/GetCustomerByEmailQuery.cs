namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomerByEmail;

/// <summary>
///     Query to retrieve a customer by their email address.
/// </summary>
public sealed record GetCustomerByEmailQuery
{
    /// <summary>
    ///     The email address of the customer to retrieve.
    /// </summary>
    public required string Email { get; init; }
}
