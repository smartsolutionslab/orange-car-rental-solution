using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a customer ID within the Reservations bounded context.
///     This is an internal representation, not a reference to Customers.Domain.Customer.CustomerIdentifier.
/// </summary>
public readonly record struct CustomerIdentifier(Guid Value) : IValueObject
{
    /// <summary>
    ///     Creates a new unique customer identifier using GUID v7.
    /// </summary>
    public static CustomerIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a new customer ID from a GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A new ReservationCustomerId instance.</returns>
    public static CustomerIdentifier From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(value));

        return new(value);
    }

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(CustomerIdentifier identifier) => identifier.Value;

    /// <summary>
    ///     Returns the string representation of the customer ID.
    /// </summary>
    public override string ToString() => Value.ToString();
}
