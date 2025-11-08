using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain;

public class VehicleTests
{
    [Fact]
    public void Create_WithValidParameters_CreatesVehicle()
    {
        // Arrange
        var name = VehicleName.Of("VW Golf");
        var category = VehicleCategory.FromCode("MITTEL");
        var location = Location.Of("BER-HBF", "Berlin Hauptbahnhof");
        var seats = SeatingCapacity.Of(5);
        var fuelType = FuelType.Petrol;
        var transmission = TransmissionType.Manual;
        var dailyRate = Money.FromGross(50.00m, 0.19m, Currency.Of("EUR"));
        var licensePlate = "B-XY-1234";

        // Act
        var vehicle = Vehicle.From(name, category, location, dailyRate, seats, fuelType, transmission);
        vehicle = vehicle.SetLicensePlate(licensePlate);

        // Assert
        vehicle.ShouldNotBeNull();
        vehicle.Name.ShouldBe(name);
        vehicle.Category.ShouldBe(category);
        vehicle.CurrentLocation.ShouldBe(location);
        vehicle.Seats.ShouldBe(seats);
        vehicle.FuelType.ShouldBe(fuelType);
        vehicle.TransmissionType.ShouldBe(transmission);
        vehicle.DailyRate.ShouldBe(dailyRate);
        vehicle.LicensePlate.ShouldBe("B-XY-1234");
        vehicle.Status.ShouldBe(VehicleStatus.Available);
    }

    [Fact]
    public void UpdateDailyRate_WithDifferentRate_UpdatesRateAndRaisesDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var oldRate = vehicle.DailyRate;
        var newRate = Money.FromGross(75.00m, 0.19m, Currency.Of("EUR"));

        // Act
        vehicle = vehicle.UpdateDailyRate(newRate);

        // Assert
        vehicle.DailyRate.ShouldBe(newRate);
        vehicle.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void UpdateDailyRate_WithSameRate_DoesNotRaiseDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var sameRate = vehicle.DailyRate;

        // Act
        var result = vehicle.UpdateDailyRate(sameRate);

        // Assert
        result.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void MoveToLocation_WithDifferentLocation_UpdatesLocationAndRaisesDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var newLocation = Location.Of("MUC-FLG", "Munich Airport");

        // Act
        vehicle = vehicle.MoveToLocation(newLocation);

        // Assert
        vehicle.CurrentLocation.ShouldBe(newLocation);
        vehicle.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void MoveToLocation_WhenVehicleIsRented_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle = vehicle.MarkAsRented();
        var newLocation = Location.Of("MUC-FLG", "Munich Airport");

        // Act & Assert
        var act = () => vehicle.MoveToLocation(newLocation);
        Should.Throw<InvalidOperationException>(act)
            .Message.ShouldBe("Cannot move a rented vehicle");
    }

    [Fact]
    public void ChangeStatus_WithDifferentStatus_UpdatesStatusAndRaisesDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Assert
        vehicle.Status.ShouldBe(VehicleStatus.Maintenance);
        vehicle.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void ChangeStatus_WithSameStatus_DoesNotRaiseDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var currentStatus = vehicle.Status;

        // Act
        var result = vehicle.ChangeStatus(currentStatus);

        // Assert
        result.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void MarkAsAvailable_WhenNotAvailable_ChangesStatusToAvailable()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle = vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Act
        vehicle = vehicle.MarkAsAvailable();

        // Assert
        vehicle.Status.ShouldBe(VehicleStatus.Available);
    }

    [Fact]
    public void MarkAsAvailable_WhenAlreadyAvailable_DoesNothing()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.MarkAsAvailable();

        // Assert
        result.Status.ShouldBe(VehicleStatus.Available);
        result.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void MarkAsRented_ChangesStatusToRented()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle = vehicle.MarkAsRented();

        // Assert
        vehicle.Status.ShouldBe(VehicleStatus.Rented);
        vehicle.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void MarkAsAvailable_AfterRented_ChangesStatusToAvailable()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle = vehicle.MarkAsRented();
        vehicle.ClearDomainEvents();

        // Act
        vehicle = vehicle.MarkAsAvailable();

        // Assert
        vehicle.Status.ShouldBe(VehicleStatus.Available);
        vehicle.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenAvailable_ChangesStatusToMaintenance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle = vehicle.MarkAsUnderMaintenance();

        // Assert
        vehicle.Status.ShouldBe(VehicleStatus.Maintenance);
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenRented_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle = vehicle.MarkAsRented();

        // Act & Assert
        var act = () => vehicle.MarkAsUnderMaintenance();
        Should.Throw<InvalidOperationException>(act)
            .Message.ShouldBe("Cannot put rented vehicle under maintenance");
    }

    [Fact]
    public void ChangeStatus_ToOutOfService_ChangesStatus()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle = vehicle.ChangeStatus(VehicleStatus.OutOfService);

        // Assert
        vehicle.Status.ShouldBe(VehicleStatus.OutOfService);
        vehicle.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void SetLicensePlate_WithValidPlate_UpdatesLicensePlate()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle = vehicle.SetLicensePlate("B-AB-9999");

        // Assert
        vehicle.LicensePlate.ShouldBe("B-AB-9999");
    }

    [Fact]
    public void SetLicensePlate_WithWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act & Assert
        var act = () => vehicle.SetLicensePlate("  ");
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("String cannot be null, empty, or whitespace");
    }

    [Fact]
    public void SetLicensePlate_WithEmpty_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act & Assert
        var act = () => vehicle.SetLicensePlate("");
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("String cannot be null, empty, or whitespace");
    }

    [Fact]
    public void SetLicensePlate_NormalizesToUpperCase()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle = vehicle.SetLicensePlate("b-xy-1234");

        // Assert
        vehicle.LicensePlate.ShouldBe("B-XY-1234");
    }

    [Fact]
    public void IsAvailableForRental_WhenAvailable_ReturnsTrue()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.ShouldBeTrue();
    }

    [Fact]
    public void IsAvailableForRental_WhenRented_ReturnsFalse()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle = vehicle.MarkAsRented();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.ShouldBeFalse();
    }

    [Fact]
    public void IsAvailableForRental_WhenInMaintenance_ReturnsFalse()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle = vehicle.MarkAsUnderMaintenance();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.ShouldBeFalse();
    }

    private Vehicle CreateTestVehicle()
    {
        var vehicle = Vehicle.From(
            VehicleName.Of("VW Golf"),
            VehicleCategory.FromCode("MITTEL"),
            Location.Of("BER-HBF", "Berlin Hauptbahnhof"),
            Money.FromGross(50.00m, 0.19m, Currency.Of("EUR")),
            SeatingCapacity.Of(5),
            FuelType.Petrol,
            TransmissionType.Manual
        );
        vehicle = vehicle.SetLicensePlate("B-XY-1234");
        vehicle.ClearDomainEvents();
        return vehicle;
    }
}
