namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Current status of a customer account.
/// </summary>
public enum CustomerStatus
{
    /// <summary>Customer account is active and can make reservations</summary>
    Active = 1,

    /// <summary>Customer account is temporarily suspended (e.g., pending payment)</summary>
    Suspended = 2,

    /// <summary>Customer account is blocked and cannot make reservations</summary>
    Blocked = 3
}
