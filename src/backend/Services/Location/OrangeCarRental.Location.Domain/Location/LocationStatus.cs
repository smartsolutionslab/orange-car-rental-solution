namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Operational status of a rental location.
/// </summary>
public enum LocationStatus
{
    /// <summary>Location is active and accepting reservations</summary>
    Active = 1,

    /// <summary>Location is temporarily closed (maintenance, renovation)</summary>
    Inactive = 2,

    /// <summary>Location is permanently closed</summary>
    Closed = 3
}
