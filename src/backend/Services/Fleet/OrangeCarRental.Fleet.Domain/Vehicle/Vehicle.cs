using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle.Events;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Vehicle aggregate root.
///     Represents a vehicle in the rental fleet with German market-specific pricing (19% VAT).
/// </summary>
public sealed class Vehicle : AggregateRoot<VehicleIdentifier>
{
    // For EF Core - properties will be set by EF Core during materialization
    private Vehicle()
    {
        Name = default!;
        Category = default!;
        CurrentLocation = default!;
        DailyRate = default;
        Seats = default!;
    }

    private Vehicle(
        VehicleIdentifier id,
        VehicleName name,
        VehicleCategory category,
        Location currentLocation,
        Money dailyRate,
        SeatingCapacity seats,
        FuelType fuelType,
        TransmissionType transmissionType)
        : base(id)
    {
        Name = name;
        Category = category;
        CurrentLocation = currentLocation;
        DailyRate = dailyRate;
        Seats = seats;
        FuelType = fuelType;
        TransmissionType = transmissionType;
        Status = VehicleStatus.Available;

        AddDomainEvent(new VehicleAddedToFleet(Id, Name, Category, DailyRate));
    }

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public VehicleName Name { get; init; }
    public VehicleCategory Category { get; init; }
    public Location CurrentLocation { get; init; }
    public Money DailyRate { get; init; }
    public SeatingCapacity Seats { get; init; }
    public FuelType FuelType { get; init; }
    public TransmissionType TransmissionType { get; init; }
    public VehicleStatus Status { get; init; }
    public string? LicensePlate { get; init; }
    public Manufacturer? Manufacturer { get; init; }
    public VehicleModel? Model { get; init; }
    public ManufacturingYear? Year { get; init; }
    public string? ImageUrl { get; init; }

    /// <summary>
    ///     Create a new vehicle with German VAT-inclusive pricing.
    /// </summary>
    public static Vehicle From(
        VehicleName name,
        VehicleCategory category,
        Location currentLocation,
        Money dailyRate,
        SeatingCapacity seats,
        FuelType fuelType,
        TransmissionType transmissionType)
    {
        return new Vehicle(
            VehicleIdentifier.New(),
            name,
            category,
            currentLocation,
            dailyRate,
            seats,
            fuelType,
            transmissionType
        );
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    ///     Does not raise domain events - caller is responsible for that.
    /// </summary>
    private Vehicle CreateMutatedCopy(
        VehicleName? name = null,
        VehicleCategory? category = null,
        Location? currentLocation = null,
        Money? dailyRate = null,
        SeatingCapacity? seats = null,
        FuelType? fuelType = null,
        TransmissionType? transmissionType = null,
        VehicleStatus? status = null,
        string? licensePlate = null,
        Manufacturer? manufacturer = null,
        VehicleModel? model = null,
        ManufacturingYear? year = null,
        string? imageUrl = null)
    {
        return new Vehicle
        {
            Id = Id,
            Name = name ?? Name,
            Category = category ?? Category,
            CurrentLocation = currentLocation ?? CurrentLocation,
            DailyRate = dailyRate ?? DailyRate,
            Seats = seats ?? Seats,
            FuelType = fuelType ?? FuelType,
            TransmissionType = transmissionType ?? TransmissionType,
            Status = status ?? Status,
            LicensePlate = licensePlate ?? LicensePlate,
            Manufacturer = manufacturer ?? Manufacturer,
            Model = model ?? Model,
            Year = year ?? Year,
            ImageUrl = imageUrl ?? ImageUrl
        };
    }

    /// <summary>
    ///     Update the daily rental rate.
    ///     Returns a new instance with the updated rate (immutable pattern).
    /// </summary>
    public Vehicle UpdateDailyRate(Money newDailyRate)
    {
        if (newDailyRate == DailyRate) return this;

        var oldRate = DailyRate;
        var updated = CreateMutatedCopy(dailyRate: newDailyRate);

        updated.AddDomainEvent(new VehicleDailyRateChanged(Id, oldRate, newDailyRate));

        return updated;
    }

    /// <summary>
    ///     Move vehicle to a different location.
    ///     Returns a new instance with the updated location (immutable pattern).
    /// </summary>
    public Vehicle MoveToLocation(Location newLocation)
    {
        if (newLocation == CurrentLocation) return this;

        if (Status == VehicleStatus.Rented) throw new InvalidOperationException("Cannot move a rented vehicle");

        var oldLocation = CurrentLocation;
        var updated = CreateMutatedCopy(currentLocation: newLocation);

        updated.AddDomainEvent(new VehicleLocationChanged(Id, oldLocation, newLocation));

        return updated;
    }

    /// <summary>
    ///     Change vehicle status.
    ///     Returns a new instance with the updated status (immutable pattern).
    /// </summary>
    public Vehicle ChangeStatus(VehicleStatus newStatus)
    {
        if (newStatus == Status) return this;

        var updated = CreateMutatedCopy(status: newStatus);
        updated.AddDomainEvent(new VehicleStatusChanged(Id, newStatus));

        return updated;
    }

    /// <summary>
    ///     Mark vehicle as available for rental.
    ///     Returns a new instance with available status (immutable pattern).
    /// </summary>
    public Vehicle MarkAsAvailable()
    {
        if (Status == VehicleStatus.Available) return this;

        return ChangeStatus(VehicleStatus.Available);
    }

    /// <summary>
    ///     Mark vehicle as rented.
    ///     Returns a new instance with rented status (immutable pattern).
    /// </summary>
    public Vehicle MarkAsRented()
    {
        if (Status != VehicleStatus.Available && Status != VehicleStatus.Reserved)
            throw new InvalidOperationException($"Cannot rent vehicle in status: {Status}");

        return ChangeStatus(VehicleStatus.Rented);
    }

    /// <summary>
    ///     Mark vehicle as under maintenance.
    ///     Returns a new instance with maintenance status (immutable pattern).
    /// </summary>
    public Vehicle MarkAsUnderMaintenance()
    {
        if (Status == VehicleStatus.Rented)
            throw new InvalidOperationException("Cannot put rented vehicle under maintenance");

        return ChangeStatus(VehicleStatus.Maintenance);
    }

    /// <summary>
    ///     Set additional details (manufacturer, model, year, image).
    ///     Returns a new instance with the updated details (immutable pattern).
    /// </summary>
    public Vehicle SetDetails(Manufacturer? manufacturer, VehicleModel? model, ManufacturingYear? year,
        string? imageUrl)
    {
        return CreateMutatedCopy(
            manufacturer: manufacturer,
            model: model,
            year: year,
            imageUrl: imageUrl?.Trim());
    }

    /// <summary>
    ///     Set license plate.
    ///     Returns a new instance with the updated license plate (immutable pattern).
    /// </summary>
    public Vehicle SetLicensePlate(string licensePlate)
    {
        Ensure.That(licensePlate, nameof(licensePlate))
            .IsNotNullOrWhiteSpace();

        return CreateMutatedCopy(licensePlate: licensePlate.ToUpperInvariant().Trim());
    }

    /// <summary>
    ///     Check if vehicle is available for rental in the given period.
    /// </summary>
    public bool IsAvailableForRental() => Status == VehicleStatus.Available;
}
