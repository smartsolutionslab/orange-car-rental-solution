using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Location aggregate root.
///     Represents a rental station where vehicles can be picked up or returned.
/// </summary>
public sealed class Location : AggregateRoot<LocationIdentifier>
{
    // For EF Core
    private Location()
    {
        Code = default;
        Name = default;
        Address = default;
        OpeningHours = default;
        Contact = default;
    }

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public LocationCode Code { get; init; }
    public LocationName Name { get; init; }
    public LocationAddress Address { get; init; }
    public GeoCoordinates? Coordinates { get; init; }
    public OpeningHours OpeningHours { get; init; }
    public ContactInfo Contact { get; init; }
    public LocationStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    ///     Creates a new rental location.
    /// </summary>
    public static Location Create(
        LocationCode code,
        LocationName name,
        LocationAddress address,
        OpeningHours openingHours,
        ContactInfo contact,
        GeoCoordinates? coordinates = null)
    {
        return new Location
        {
            Id = LocationIdentifier.New(),
            Code = code,
            Name = name,
            Address = address,
            Coordinates = coordinates,
            OpeningHours = openingHours,
            Contact = contact,
            Status = LocationStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    /// </summary>
    private Location CreateMutatedCopy(
        LocationName? name = null,
        LocationAddress? address = null,
        GeoCoordinates? coordinates = null,
        OpeningHours? openingHours = null,
        ContactInfo? contact = null,
        LocationStatus? status = null)
    {
        return new Location
        {
            Id = Id,
            Code = Code,
            Name = name ?? Name,
            Address = address ?? Address,
            Coordinates = coordinates ?? Coordinates,
            OpeningHours = openingHours ?? OpeningHours,
            Contact = contact ?? Contact,
            Status = status ?? Status,
            CreatedAt = CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     Updates location details.
    ///     Returns a new instance with updated values (immutable pattern).
    /// </summary>
    public Location UpdateDetails(
        LocationName name,
        LocationAddress address,
        OpeningHours openingHours,
        ContactInfo contact,
        GeoCoordinates? coordinates = null)
    {
        return CreateMutatedCopy(
            name: name,
            address: address,
            openingHours: openingHours,
            contact: contact,
            coordinates: coordinates);
    }

    /// <summary>
    ///     Activates the location.
    ///     Returns a new instance with active status (immutable pattern).
    /// </summary>
    public Location Activate()
    {
        return CreateMutatedCopy(status: LocationStatus.Active);
    }

    /// <summary>
    ///     Deactivates the location (temporarily closed).
    ///     Returns a new instance with inactive status (immutable pattern).
    /// </summary>
    public Location Deactivate()
    {
        return CreateMutatedCopy(status: LocationStatus.Inactive);
    }

    /// <summary>
    ///     Closes the location permanently.
    ///     Returns a new instance with closed status (immutable pattern).
    /// </summary>
    public Location Close()
    {
        return CreateMutatedCopy(status: LocationStatus.Closed);
    }
}
