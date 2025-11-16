namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a vehicle ID within the Reservations bounded context.
///     This is an internal representation, not a reference to Fleet.Domain.Vehicle.VehicleIdentifier.
/// </summary>
public readonly record struct ReservationVehicleId(Guid Value)
{
    /// <summary>
    ///     Creates a new vehicle ID from a GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A new ReservationVehicleId instance.</returns>
    public static ReservationVehicleId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(value));

        return new(value);
    }

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(ReservationVehicleId id) => id.Value;

    /// <summary>
    ///     Returns the string representation of the vehicle ID.
    /// </summary>
    public override string ToString() => Value.ToString();
}
