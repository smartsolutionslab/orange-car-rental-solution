using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle.Events;
using SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.Entities;

public class VehicleTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateVehicle()
    {
        // Arrange & Act
        var vehicle = VehicleBuilder.Default().Build();

        // Assert
        vehicle.ShouldNotBeNull();
        vehicle.Id.Value.ShouldNotBe(Guid.Empty);
        vehicle.Status.ShouldBe(VehicleStatus.Available);
    }

    [Fact]
    public void From_ShouldRaiseVehicleAddedToFleetEvent()
    {
        // Arrange & Act
        var vehicle = VehicleBuilder.Default().Build();

        // Assert
        var events = vehicle.DomainEvents;
        events.ShouldNotBeEmpty();
        var addedEvent = events.ShouldHaveSingleItem();
        addedEvent.ShouldBeOfType<VehicleAddedToFleet>();
    }

    [Fact]
    public void UpdateDailyRate_WithDifferentRate_ShouldReturnNewInstance()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();
        var newRate = Money.Euro(99.99m);

        // Act
        var updatedVehicle = vehicle.UpdateDailyRate(newRate);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance (immutable)
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.DailyRate.ShouldBe(newRate);
    }

    [Fact]
    public void UpdateDailyRate_WithSameRate_ShouldReturnSameInstance()
    {
        // Arrange
        var dailyRate = Money.Euro(89.99m);
        var vehicle = VehicleBuilder.Default()
            .WithDailyRate(89.99m)
            .Build();

        // Act
        var updatedVehicle = vehicle.UpdateDailyRate(dailyRate);

        // Assert
        updatedVehicle.ShouldBeSameAs(vehicle); // Same instance if no change
    }

    [Fact]
    public void UpdateDailyRate_ShouldRaiseVehicleDailyRateChangedEvent()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();
        vehicle.ClearDomainEvents(); // Clear creation event
        var newRate = Money.Euro(99.99m);

        // Act
        var updatedVehicle = vehicle.UpdateDailyRate(newRate);

        // Assert
        var events = updatedVehicle.DomainEvents;
        events.ShouldNotBeEmpty();
        var rateChangedEvent = events.ShouldHaveSingleItem();
        rateChangedEvent.ShouldBeOfType<VehicleDailyRateChanged>();
    }

    [Fact]
    public void MoveToLocation_WithDifferentLocation_ShouldReturnNewInstance()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();
        var newLocation = Locations.MunichAirport;

        // Act
        var updatedVehicle = vehicle.MoveToLocation(newLocation);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance (immutable)
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.CurrentLocationCode.ShouldBe(newLocation);
    }

    [Fact]
    public void MoveToLocation_WithSameLocation_ShouldReturnSameInstance()
    {
        // Arrange
        var location = Locations.BerlinHauptbahnhof;
        var vehicle = VehicleBuilder.Default()
            .AtLocation(location)
            .Build();

        // Act
        var updatedVehicle = vehicle.MoveToLocation(location);

        // Assert
        updatedVehicle.ShouldBeSameAs(vehicle); // Same instance if no change
    }

    [Fact]
    public void MoveToLocation_WithRentedVehicle_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rentedVehicle = VehicleBuilder.Default().BuildRented();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            rentedVehicle.MoveToLocation(Locations.MunichAirport));
        ex.Message.ShouldContain("Cannot move a rented vehicle");
    }

    [Fact]
    public void MoveToLocation_ShouldRaiseVehicleLocationChangedEvent()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();
        vehicle.ClearDomainEvents(); // Clear creation event
        var newLocation = Locations.MunichAirport;

        // Act
        var updatedVehicle = vehicle.MoveToLocation(newLocation);

        // Assert
        var events = updatedVehicle.DomainEvents;
        events.ShouldNotBeEmpty();
        var locationChangedEvent = events.ShouldHaveSingleItem();
        locationChangedEvent.ShouldBeOfType<VehicleLocationChanged>();
    }

    [Fact]
    public void ChangeStatus_WithDifferentStatus_ShouldReturnNewInstance()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var updatedVehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Assert
        updatedVehicle.ShouldNotBeSameAs(vehicle); // New instance (immutable)
        updatedVehicle.Id.ShouldBe(vehicle.Id); // Same ID
        updatedVehicle.Status.ShouldBe(VehicleStatus.Maintenance);
    }

    [Fact]
    public void ChangeStatus_WithSameStatus_ShouldReturnSameInstance()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var updatedVehicle = vehicle.ChangeStatus(VehicleStatus.Available);

        // Assert
        updatedVehicle.ShouldBeSameAs(vehicle); // Same instance if no change
    }

    [Fact]
    public void ChangeStatus_ShouldRaiseVehicleStatusChangedEvent()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();
        vehicle.ClearDomainEvents(); // Clear creation event

        // Act
        var updatedVehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Assert
        var events = updatedVehicle.DomainEvents;
        events.ShouldNotBeEmpty();
        var statusChangedEvent = events.ShouldHaveSingleItem();
        statusChangedEvent.ShouldBeOfType<VehicleStatusChanged>();
    }

    [Fact]
    public void MarkAsAvailable_WhenNotAvailable_ShouldChangeToAvailable()
    {
        // Arrange
        var maintenanceVehicle = VehicleBuilder.Default().BuildInMaintenance();

        // Act
        var availableVehicle = maintenanceVehicle.MarkAsAvailable();

        // Assert
        availableVehicle.Status.ShouldBe(VehicleStatus.Available);
    }

    [Fact]
    public void MarkAsAvailable_WhenAlreadyAvailable_ShouldReturnSameInstance()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var availableVehicle = vehicle.MarkAsAvailable();

        // Assert
        availableVehicle.ShouldBeSameAs(vehicle); // Already available
    }

    [Fact]
    public void MarkAsRented_WhenAvailable_ShouldChangeToRented()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var rentedVehicle = vehicle.MarkAsRented();

        // Assert
        rentedVehicle.Status.ShouldBe(VehicleStatus.Rented);
    }

    [Fact]
    public void MarkAsRented_WhenReserved_ShouldChangeToRented()
    {
        // Arrange
        var reservedVehicle = VehicleBuilder.Default().BuildReserved();

        // Act
        var rentedVehicle = reservedVehicle.MarkAsRented();

        // Assert
        rentedVehicle.Status.ShouldBe(VehicleStatus.Rented);
    }

    [Fact]
    public void MarkAsRented_WhenMaintenance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var maintenanceVehicle = VehicleBuilder.Default().BuildInMaintenance();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            maintenanceVehicle.MarkAsRented());
        ex.Message.ShouldContain("Cannot rent vehicle in status");
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenAvailable_ShouldChangeToMaintenance()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var maintenanceVehicle = vehicle.MarkAsUnderMaintenance();

        // Assert
        maintenanceVehicle.Status.ShouldBe(VehicleStatus.Maintenance);
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenRented_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rentedVehicle = VehicleBuilder.Default().BuildRented();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            rentedVehicle.MarkAsUnderMaintenance());
        ex.Message.ShouldContain("Cannot put rented vehicle under maintenance");
    }

    [Fact]
    public void SetDetails_ShouldReturnNewInstanceWithDetails()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();
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
        var vehicle = VehicleBuilder.Default().Build();

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
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var updatedVehicle = vehicle.SetLicensePlate("b-ab 1234");

        // Assert
        updatedVehicle.LicensePlate.ShouldBe("B-AB 1234");
    }

    [Fact]
    public void SetLicensePlate_ShouldTrimWhitespace()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act
        var updatedVehicle = vehicle.SetLicensePlate("  B-AB 1234  ");

        // Assert
        updatedVehicle.LicensePlate.ShouldBe("B-AB 1234");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SetLicensePlate_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidPlate)
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => vehicle.SetLicensePlate(invalidPlate));
    }

    [Fact]
    public void SetLicensePlate_WithNull_ShouldThrowArgumentException()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => vehicle.SetLicensePlate(null!));
    }

    [Fact]
    public void IsAvailableForRental_WhenAvailable_ShouldReturnTrue()
    {
        // Arrange
        var vehicle = VehicleBuilder.Default().Build();

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
        var vehicle = VehicleBuilder.Default().Build();
        var updatedVehicle = vehicle.ChangeStatus(status);

        // Act
        var isAvailable = updatedVehicle.IsAvailableForRental();

        // Assert
        isAvailable.ShouldBeFalse();
    }

    #region Named Vehicle Tests

    [Fact]
    public void BmwX5_CreatesCorrectVehicle()
    {
        // Arrange & Act
        var vehicle = VehicleBuilder.BmwX5().Build();

        // Assert
        vehicle.Name.Value.ShouldBe("BMW X5");
        vehicle.Category.ShouldBe(VehicleCategory.SUV);
        vehicle.FuelType.ShouldBe(FuelType.Electric);
    }

    [Fact]
    public void VwGolf_CreatesCompactCar()
    {
        // Arrange & Act
        var vehicle = VehicleBuilder.VwGolf().Build();

        // Assert
        vehicle.Name.Value.ShouldBe("VW Golf");
        vehicle.Category.ShouldBe(VehicleCategory.Kompaktklasse);
        vehicle.FuelType.ShouldBe(FuelType.Petrol);
    }

    [Fact]
    public void TeslaModel3_CreatesElectricCar()
    {
        // Arrange & Act
        var vehicle = VehicleBuilder.TeslaModel3().Build();

        // Assert
        vehicle.Name.Value.ShouldBe("Tesla Model 3");
        vehicle.FuelType.ShouldBe(FuelType.Electric);
        vehicle.TransmissionType.ShouldBe(TransmissionType.Automatic);
    }

    #endregion
}
