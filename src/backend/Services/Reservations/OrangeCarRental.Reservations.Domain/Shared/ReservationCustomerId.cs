namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a customer ID within the Reservations bounded context.
///     This is an internal representation, not a reference to Customers.Domain.Customer.CustomerIdentifier.
/// </summary>
public readonly record struct ReservationCustomerId
{
    private ReservationCustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(value));

        Value = value;
    }

    /// <summary>
    ///     The underlying GUID value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    ///     Creates a new customer ID from a GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A new ReservationCustomerId instance.</returns>
    public static ReservationCustomerId From(Guid value) => new(value);

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(ReservationCustomerId id) => id.Value;

    /// <summary>
    ///     Returns the string representation of the customer ID.
    /// </summary>
    public override string ToString() => Value.ToString();
}
