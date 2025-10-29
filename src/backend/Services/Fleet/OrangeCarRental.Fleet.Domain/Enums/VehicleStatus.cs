namespace SmartSolutionsLab.Fleet.Domain.Enums;

/// <summary>
/// Current status of a vehicle.
/// </summary>
public enum VehicleStatus
{
    /// <summary>Vehicle is available for rental</summary>
    Available = 1,

    /// <summary>Vehicle is currently rented</summary>
    Rented = 2,

    /// <summary>Vehicle is under maintenance</summary>
    Maintenance = 3,

    /// <summary>Vehicle is out of service</summary>
    OutOfService = 4,

    /// <summary>Vehicle is reserved for future rental</summary>
    Reserved = 5
}
