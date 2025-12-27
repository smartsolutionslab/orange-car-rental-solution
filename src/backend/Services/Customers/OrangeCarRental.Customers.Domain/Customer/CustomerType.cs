namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Type of customer account.
/// </summary>
public enum CustomerType
{
    /// <summary>
    ///     Individual private customer (Privatkunde).
    /// </summary>
    Individual = 1,

    /// <summary>
    ///     Business customer with company account (Geschäftskunde).
    /// </summary>
    Business = 2
}
