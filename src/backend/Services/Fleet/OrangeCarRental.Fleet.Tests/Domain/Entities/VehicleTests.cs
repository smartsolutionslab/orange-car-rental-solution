using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle.Events;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.Entities;

public class VehicleTests
{
    private readonly VehicleName _validName = VehicleName.Of("BMW X5");
    private readonly VehicleCategory _validCategory = VehicleCategory.SUV;
    private readonly Location _validLocation = Location.BerlinHauptbahnhof;
    private readonly Money _validDailyRate = Money.Of(89.99m, Currency.EUR);
    private readonly SeatingCapacity _validSeats = SeatingCapacity.Of(5);
    private readonly FuelType _validFuelType = FuelType.Diesel;
    private readonly TransmissionType _validTransmission = TransmissionType.Automatic;

    [Fact]
    public void From_WithValidData_ShouldCreateVehicle()
    {
        // Act
        var vehicle = Vehicle.From(
            _validName,
            _validCategory,
            _validLocation,
            _validDailyRate,
            _validSeats,
            _validFuelType,
            _validTransmission);

        // Assert
        vehicle.ShouldNotBeNull();
        vehicle.Id.Value.ShouldNotBe(Guid.Empty);
        vehicle.Name.ShouldBe(_validName);
        vehicle.Category.ShouldBe(_validCategory);
        vehicle.CurrentLocation.ShouldBe(_validLocation);
        vehicle.DailyRate.ShouldBe(_validDailyRate);
        vehicle.Seats.ShouldBe(_validSeats);
        vehicle.FuelType.ShouldBe(_validFuelType);
        vehicle.TransmissionType.ShouldBe(_validTransmission);
        vehicle.Status.ShouldBe(VehicleStatus.Available);
    }

    [Fact]
    public void From_ShouldRaiseVehicleAddedToFleetEvent()
    {
        // Act
        var vehicle = Vehicle.From(
            _validName,
            _validCategory,
            _validLocation,
            _validDailyRate,
            _validSeats,
            _validFuelType,
            _validTransmission);

        // Assert
        var events = vehicle.GetDomainEvents();
        events.ShouldNotBeEmpty();
        var addedEvent = events.ShouldHaveSingleItem();
        addedEvent.ShouldBeOfType<VehicleAddedToFleet>();

        var evt = (VehicleAddedToFleet)addedEvent;
        evt.VehicleId.ShouldBe(vehicle.Id);
        evt.Name.ShouldBe(_validName);
        evt.Category.ShouldBe(_validCategory);
        evt.DailyRate.ShouldBe(_validDailyRate);
    }

    [Fact]
    public void UpdateDailyRate_WithDifferentRate_ShouldReturnNewInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var newRate = Money.Of(99.99m, Currency.EUR);

        // Act
        var updatedVehicle = vehicle.UpdateDailyRate(newRate);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance (immutable)
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.DailyRate.ShouldBe(newRate);
        vehicle.DailyRate.ShouldBe(_validDailyRate); // Original unchanged
    }

    [Fact]
    public void UpdateDailyRate_WithSameRate_ShouldReturnSameInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.UpdateDailyRate(_validDailyRate);

        // Assert
        updatedVehicle.ShouldBeSameAs(vehicle); // Same instance if no change
    }

    [Fact]
    public void UpdateDailyRate_ShouldRaiseVehicleDailyRateChangedEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.ClearDomainEvents(); // Clear creation event
        var newRate = Money.Of(99.99m, Currency.EUR);

        // Act
        var updatedVehicle = vehicle.UpdateDailyRate(newRate);

        // Assert
        var events = updatedVehicle.GetDomainEvents();
        events.ShouldNotBeEmpty();
        var rateChangedEvent = events.ShouldHaveSingleItem();
        rateChangedEvent.ShouldBeOfType<VehicleDailyRateChanged>();

        var evt = (VehicleDailyRateChanged)rateChangedEvent;
        evt.VehicleId.ShouldBe(vehicle.Id);
        evt.OldRate.ShouldBe(_validDailyRate);
        evt.NewRate.ShouldBe(newRate);
    }

    [Fact]
    public void MoveToLocation_WithDifferentLocation_ShouldReturnNewInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var newLocation = Location.MunichFlughafen;

        // Act
        var updatedVehicle = vehicle.MoveToLocation(newLocation);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance (immutable)
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.CurrentLocation.ShouldBe(newLocation);
        vehicle.CurrentLocation.ShouldBe(_validLocation); // Original unchanged
    }

    [Fact]
    public void MoveToLocation_WithSameLocation_ShouldReturnSameInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.MoveToLocation(_validLocation);

        // Assert
        updatedVehicle.ShouldBeSameAs(vehicle); // Same instance if no change
    }

    [Fact]
    public void MoveToLocation_WithRentedVehicle_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var rentedVehicle = vehicle.MarkAsRented();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            rentedVehicle.MoveToLocation(Location.MunichFlughafen));
        ex.Message.ShouldContain("Cannot move a rented vehicle");
    }

    [Fact]
    public void MoveToLocation_ShouldRaiseVehicleLocationChangedEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.ClearDomainEvents(); // Clear creation event
        var newLocation = Location.MunichFlughafen;

        // Act
        var updatedVehicle = vehicle.MoveToLocation(newLocation);

        // Assert
        var events = updatedVehicle.GetDomainEvents();
        events.ShouldNotBeEmpty();
        var locationChangedEvent = events.ShouldHaveSingleItem();
        locationChangedEvent.ShouldBeOfType<VehicleLocationChanged>();

        var evt = (VehicleLocationChanged)locationChangedEvent;
        evt.VehicleId.ShouldBe(vehicle.Id);
        evt.OldLocation.ShouldBe(_validLocation);
        evt.NewLocation.ShouldBe(newLocation);
    }

    [Fact]
    public void ChangeStatus_WithDifferentStatus_ShouldReturnNewInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance (immutable)
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.Status.ShouldBe(VehicleStatus.Maintenance);
        vehicle.Status.ShouldBe(VehicleStatus.Available); // Original unchanged
    }

    [Fact]
    public void ChangeStatus_WithSameStatus_ShouldReturnSameInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.ChangeStatus(VehicleStatus.Available);

        // Assert
        updatedVehicle.ShouldBeSameAs(vehicle); // Same instance if no change
    }

    [Fact]
    public void ChangeStatus_ShouldRaiseVehicleStatusChangedEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.ClearDomainEvents(); // Clear creation event

        // Act
        var updatedVehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Assert
        var events = updatedVehicle.GetDomainEvents();
        events.ShouldNotBeEmpty();
        var statusChangedEvent = events.ShouldHaveSingleItem();
        statusChangedEvent.ShouldBeOfType<VehicleStatusChanged>();

        var evt = (VehicleStatusChanged)statusChangedEvent;
        evt.VehicleId.ShouldBe(vehicle.Id);
        evt.NewStatus.ShouldBe(VehicleStatus.Maintenance);
    }

    [Fact]
    public void MarkAsAvailable_WhenNotAvailable_ShouldChangeToAvailable()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var maintenanceVehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Act
        var availableVehicle = maintenanceVehicle.MarkAsAvailable();

        // Assert
        availableVehicle.Status.ShouldBe(VehicleStatus.Available);
    }

    [Fact]
    public void MarkAsAvailable_WhenAlreadyAvailable_ShouldReturnSameInstance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var availableVehicle = vehicle.MarkAsAvailable();

        // Assert
        availableVehicle.ShouldBeSameAs(vehicle); // Already available
    }

    [Fact]
    public void MarkAsRented_WhenAvailable_ShouldChangeToRented()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var rentedVehicle = vehicle.MarkAsRented();

        // Assert
        rentedVehicle.Status.ShouldBe(VehicleStatus.Rented);
    }

    [Fact]
    public void MarkAsRented_WhenReserved_ShouldChangeToRented()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var reservedVehicle = vehicle.ChangeStatus(VehicleStatus.Reserved);

        // Act
        var rentedVehicle = reservedVehicle.MarkAsRented();

        // Assert
        rentedVehicle.Status.ShouldBe(VehicleStatus.Rented);
    }

    [Fact]
    public void MarkAsRented_WhenMaintenance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var maintenanceVehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            maintenanceVehicle.MarkAsRented());
        ex.Message.ShouldContain("Cannot rent vehicle in status");
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenAvailable_ShouldChangeToMaintenance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var maintenanceVehicle = vehicle.MarkAsUnderMaintenance();

        // Assert
        maintenanceVehicle.Status.ShouldBe(VehicleStatus.Maintenance);
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenRented_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var rentedVehicle = vehicle.MarkAsRented();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            rentedVehicle.MarkAsUnderMaintenance());
        ex.Message.ShouldContain("Cannot put rented vehicle under maintenance");
    }

    [Fact]
    public void SetDetails_ShouldReturnNewInstanceWithDetails()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var manufacturer = Manufacturer.Of("BMW");
        var model = VehicleModel.Of("X5 xDrive40i");
        var year = ManufacturingYear.Of(2024);
        var imageUrl = "https://example.com/bmw-x5.jpg";

        // Act
        var updatedVehicle = vehicle.SetDetails(manufacturer, model, year, imageUrl);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.Manufacturer.ShouldBe(manufacturer);
        updatedVehicle.Model.ShouldBe(model);
        updatedVehicle.Year.ShouldBe(year);
        updatedVehicle.ImageUrl.ShouldBe(imageUrl);
    }

    [Fact]
    public void SetLicensePlate_ShouldReturnNewInstanceWithLicensePlate()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.SetLicensePlate("B-AB 1234");

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.LicensePlate.ShouldBe("B-AB 1234");
    }

    [Fact]
    public void SetLicensePlate_ShouldConvertToUpperCase()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.SetLicensePlate("b-ab 1234");

        // Assert
        updatedVehicle.LicensePlate.ShouldBe("B-AB 1234");
    }

    [Fact]
    public void SetLicensePlate_ShouldTrimWhitespace()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var updatedVehicle = vehicle.SetLicensePlate("  B-AB 1234  ");

        // Assert
        updatedVehicle.LicensePlate.ShouldBe("B-AB 1234");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void SetLicensePlate_WithNullOrWhitespace_ShouldThrowArgumentException(string invalidPlate)
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act & Assert
        Should.Throw<ArgumentException>(() => vehicle.SetLicensePlate(invalidPlate));
    }

    [Fact]
    public void IsAvailableForRental_WhenAvailable_ShouldReturnTrue()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.ShouldBeTrue();
    }

    [Theory]
    [InlineData(VehicleStatus.Rented)]
    [InlineData(VehicleStatus.Maintenance)]
    [InlineData(VehicleStatus.OutOfService)]
    [InlineData(VehicleStatus.Reserved)]
    public void IsAvailableForRental_WhenNotAvailable_ShouldReturnFalse(VehicleStatus status)
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var updatedVehicle = vehicle.ChangeStatus(status);

        // Act
        var isAvailable = updatedVehicle.IsAvailableForRental();

        // Assert
        isAvailable.ShouldBeFalse();
    }

    private Vehicle CreateTestVehicle()
    {
        return Vehicle.From(
            _validName,
            _validCategory,
            _validLocation,
            _validDailyRate,
            _validSeats,
            _validFuelType,
            _validTransmission);
    }
}
