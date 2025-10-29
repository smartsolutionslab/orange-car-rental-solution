using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Events;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;

/// <summary>
/// Vehicle aggregate root.
/// Represents a vehicle in the rental fleet with German market-specific pricing (19% VAT).
/// </summary>
public sealed class Vehicle : AggregateRoot<VehicleIdentifier>
{
    public VehicleName Name { get; private set; }
    public VehicleCategory Category { get; private set; }
    public Location CurrentLocation { get; private set; }
    public Money DailyRate { get; private set; }
    public int Seats { get; private set; }
    public FuelType FuelType { get; private set; }
    public TransmissionType TransmissionType { get; private set; }
    public VehicleStatus Status { get; private set; }
    public string? LicensePlate { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public int? Year { get; private set; }
    public string? ImageUrl { get; private set; }

    // For EF Core - properties will be set by EF Core during materialization
    private Vehicle() : base(VehicleIdentifier.New())
    {
        Name = null!;
        Category = null!;
        CurrentLocation = null!;
        DailyRate = null!;
    }

    private Vehicle(
        VehicleIdentifier id,
        VehicleName name,
        VehicleCategory category,
        Location currentLocation,
        Money dailyRate,
        int seats,
        FuelType fuelType,
        TransmissionType transmissionType)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        CurrentLocation = currentLocation ?? throw new ArgumentNullException(nameof(currentLocation));
        DailyRate = dailyRate ?? throw new ArgumentNullException(nameof(dailyRate));
        Seats = seats > 0 && seats <= 9
            ? seats
            : throw new ArgumentException("Seats must be between 1 and 9", nameof(seats));
        FuelType = fuelType;
        TransmissionType = transmissionType;
        Status = VehicleStatus.Available;

        AddDomainEvent(new VehicleAddedToFleet(Id, Name, Category, DailyRate));
    }

    /// <summary>
    /// Create a new vehicle with German VAT-inclusive pricing.
    /// </summary>
    public static Vehicle Create(
        VehicleName name,
        VehicleCategory category,
        Location currentLocation,
        Money dailyRate,
        int seats,
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
    /// Update the daily rental rate.
    /// </summary>
    public void UpdateDailyRate(Money newDailyRate)
    {
        if (newDailyRate is null)
        {
            throw new ArgumentNullException(nameof(newDailyRate));
        }

        if (newDailyRate == DailyRate)
        {
            return;
        }

        var oldRate = DailyRate;
        DailyRate = newDailyRate;

        AddDomainEvent(new VehicleDailyRateChanged(Id, oldRate, newDailyRate));
    }

    /// <summary>
    /// Move vehicle to a different location.
    /// </summary>
    public void MoveToLocation(Location newLocation)
    {
        if (newLocation is null)
        {
            throw new ArgumentNullException(nameof(newLocation));
        }

        if (newLocation == CurrentLocation)
        {
            return;
        }

        if (Status == VehicleStatus.Rented)
        {
            throw new InvalidOperationException("Cannot move a rented vehicle");
        }

        var oldLocation = CurrentLocation;
        CurrentLocation = newLocation;

        AddDomainEvent(new VehicleLocationChanged(Id, oldLocation, newLocation));
    }

    /// <summary>
    /// Change vehicle status.
    /// </summary>
    public void ChangeStatus(VehicleStatus newStatus)
    {
        if (newStatus == Status)
        {
            return;
        }

        Status = newStatus;
        AddDomainEvent(new VehicleStatusChanged(Id, Status));
    }

    /// <summary>
    /// Mark vehicle as available for rental.
    /// </summary>
    public void MarkAsAvailable()
    {
        if (Status == VehicleStatus.Available)
        {
            return;
        }

        ChangeStatus(VehicleStatus.Available);
    }

    /// <summary>
    /// Mark vehicle as rented.
    /// </summary>
    public void MarkAsRented()
    {
        if (Status != VehicleStatus.Available && Status != VehicleStatus.Reserved)
        {
            throw new InvalidOperationException($"Cannot rent vehicle in status: {Status}");
        }

        ChangeStatus(VehicleStatus.Rented);
    }

    /// <summary>
    /// Mark vehicle as under maintenance.
    /// </summary>
    public void MarkAsUnderMaintenance()
    {
        if (Status == VehicleStatus.Rented)
        {
            throw new InvalidOperationException("Cannot put rented vehicle under maintenance");
        }

        ChangeStatus(VehicleStatus.Maintenance);
    }

    /// <summary>
    /// Set additional details (manufacturer, model, year, image).
    /// </summary>
    public void SetDetails(string? manufacturer, string? model, int? year, string? imageUrl)
    {
        Manufacturer = manufacturer?.Trim();
        Model = model?.Trim();
        Year = year;
        ImageUrl = imageUrl?.Trim();
    }

    /// <summary>
    /// Set license plate.
    /// </summary>
    public void SetLicensePlate(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("License plate cannot be empty", nameof(licensePlate));
        }

        LicensePlate = licensePlate.ToUpperInvariant().Trim();
    }

    /// <summary>
    /// Check if vehicle is available for rental in the given period.
    /// </summary>
    public bool IsAvailableForRental()
    {
        return Status == VehicleStatus.Available;
    }
}
