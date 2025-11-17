using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a vehicle ID within the Reservations bounded context.
///     This is an internal representation, not a reference to Fleet.Domain.Vehicle.VehicleIdentifier.
/// </summary>
public readonly record struct VehicleIdentifier(Guid Value) : IValueObject
{
    /// <summary>
    ///     Creates a new unique vehicle identifier using GUID v7.
    /// </summary>
    public static VehicleIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a new vehicle ID from a GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A new ReservationVehicleId instance.</returns>
    public static VehicleIdentifier From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(value));

        return new(value);
    }

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(VehicleIdentifier identifier) => identifier.Value;

    /// <summary>
    ///     Returns the string representation of the vehicle ID.
    /// </summary>
    public override string ToString() => Value.ToString();
}
