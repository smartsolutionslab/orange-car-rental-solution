using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location.Events;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Location aggregate root.
///     Represents a rental station location where vehicles can be picked up or returned.
///     Uses LocationCode as the natural key/identity.
/// </summary>
public sealed class Location : AggregateRoot<LocationCode>
{
    // For EF Core - properties will be set by EF Core during materialization
    private Location()
    {
        Name = default!;
        Address = default!;
    }

    private Location(
        LocationCode code,
        LocationName name,
        Address address,
        LocationStatus status = LocationStatus.Active)
        : base(code)
    {
        Name = name;
        Address = address;
        Status = status;

        AddDomainEvent(new LocationAdded(Code, Name));
    }

    public void Deconstruct(out LocationCode code, out LocationName name, out Address address, out LocationStatus status)
    {
        code = Code;
        name = Name;
        address = Address;
        status = Status;
    }

    /// <summary>
    ///     The location code (natural key/identity). Alias for Id.
    /// </summary>
    public LocationCode Code => Id;

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public LocationName Name { get; init; }
    public Address Address { get; init; }
    public LocationStatus Status { get; init; }

    /// <summary>
    ///     Creates a new location.
    /// </summary>
    public static Location Create(
        LocationCode code,
        LocationName name,
        Address address)
    {
        return new Location(
            code,
            name,
            address,
            LocationStatus.Active
        );
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    ///     Does not raise domain events - caller is responsible for that.
    /// </summary>
    private Location CreateMutatedCopy(
        LocationName? name = null,
        Address? address = null,
        LocationStatus? status = null)
    {
        return new Location
        {
            Id = Id,
            Name = name ?? Name,
            Address = address ?? Address,
            Status = status ?? Status
        };
    }

    /// <summary>
    ///     Updates the location information.
    /// </summary>
    public Location UpdateInformation(LocationName name, Address address)
    {
        var updated = CreateMutatedCopy(name: name, address: address);
        updated.AddDomainEvent(new LocationInformationUpdated(Code, Name, Address));
        return updated;
    }

    /// <summary>
    ///     Changes the status of the location.
    /// </summary>
    public Location ChangeStatus(LocationStatus newStatus, StatusChangeReason? reason = null)
    {
        if (newStatus == Status) return this;

        var updated = CreateMutatedCopy(status: newStatus);
        updated.AddDomainEvent(new LocationStatusChanged(Code, Status, newStatus, reason));

        return updated;
    }

    /// <summary>
    ///     Activates the location.
    /// </summary>
    public Location Activate() => ChangeStatus(LocationStatus.Active);

    /// <summary>
    ///     Closes the location temporarily.
    /// </summary>
    public Location Close(StatusChangeReason? reason = null) => ChangeStatus(LocationStatus.Closed, reason);

    /// <summary>
    ///     Marks the location as under maintenance.
    /// </summary>
    public Location MarkUnderMaintenance(StatusChangeReason? reason = null) => ChangeStatus(LocationStatus.UnderMaintenance, reason);

    /// <summary>
    ///     Deactivates the location permanently.
    /// </summary>
    public Location Deactivate(StatusChangeReason? reason = null) => ChangeStatus(LocationStatus.Inactive, reason);

    public override string ToString() => $"{Name.Value} ({Code.Value})";
}
