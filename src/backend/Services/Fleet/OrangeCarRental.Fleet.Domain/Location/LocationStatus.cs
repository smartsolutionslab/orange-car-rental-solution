namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Current status of a rental location.
/// </summary>
public enum LocationStatus
{
    /// <summary>Location is active and operational</summary>
    Active = 1,

    /// <summary>Location is temporarily closed</summary>
    Closed = 2,

    /// <summary>Location is under maintenance</summary>
    UnderMaintenance = 3,

    /// <summary>Location is permanently closed</summary>
    Inactive = 4
}
