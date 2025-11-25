using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location.Events;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Location aggregate root.
///     Represents a rental station location where vehicles can be picked up or returned.
/// </summary>
public sealed class Location : AggregateRoot<LocationIdentifier>
{
    // For EF Core - properties will be set by EF Core during materialization
    private Location()
    {
        Code = default!;
        Name = default!;
        Address = default!;
    }

    private Location(
        LocationIdentifier id,
        LocationCode code,
        LocationName name,
        Address address,
        LocationStatus status = LocationStatus.Active)
        : base(id)
    {
        Code = code;
        Name = name;
        Address = address;
        Status = status;

        AddDomainEvent(new LocationAdded(Id, Code, Name));
    }

    public void Deconstruct(out LocationIdentifier id, out LocationCode code, out LocationName name, out Address address, out LocationStatus status)
    {
        id = Id;
        code = Code;
        name = Name;
        address = Address;
        status = Status;
    }

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public LocationCode Code { get; init; }
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
            LocationIdentifier.New(),
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
        LocationCode? code = null,
        LocationName? name = null,
        Address? address = null,
        LocationStatus? status = null)
    {
        return new Location
        {
            Id = Id,
            Code = code ?? Code,
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
        updated.AddDomainEvent(new LocationInformationUpdated(Id, Name, Address));
        return updated;
    }

    /// <summary>
    ///     Changes the status of the location.
    /// </summary>
    public Location ChangeStatus(LocationStatus newStatus, string? reason = null)
    {
        if (newStatus == Status) return this;

        var updated = CreateMutatedCopy(status: newStatus);
        updated.AddDomainEvent(new LocationStatusChanged(Id, Code, Status, newStatus, reason));

        return updated;
    }

    /// <summary>
    ///     Activates the location.
    /// </summary>
    public Location Activate() => ChangeStatus(LocationStatus.Active);

    /// <summary>
    ///     Closes the location temporarily.
    /// </summary>
    public Location Close(string? reason = null) => ChangeStatus(LocationStatus.Closed, reason);

    /// <summary>
    ///     Marks the location as under maintenance.
    /// </summary>
    public Location MarkUnderMaintenance(string? reason = null) => ChangeStatus(LocationStatus.UnderMaintenance, reason);

    /// <summary>
    ///     Deactivates the location permanently.
    /// </summary>
    public Location Deactivate(string? reason = null) => ChangeStatus(LocationStatus.Inactive, reason);

    public override string ToString() => $"{Name.Value} ({Code.Value})";
}
