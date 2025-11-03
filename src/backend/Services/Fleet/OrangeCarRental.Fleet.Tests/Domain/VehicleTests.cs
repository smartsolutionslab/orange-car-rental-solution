using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

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
        vehicle.SetLicensePlate(licensePlate);

        // Assert
        vehicle.Should().NotBeNull();
        vehicle.Name.Should().Be(name);
        vehicle.Category.Should().Be(category);
        vehicle.CurrentLocation.Should().Be(location);
        vehicle.Seats.Should().Be(seats);
        vehicle.FuelType.Should().Be(fuelType);
        vehicle.TransmissionType.Should().Be(transmission);
        vehicle.DailyRate.Should().Be(dailyRate);
        vehicle.LicensePlate.Should().Be("B-XY-1234");
        vehicle.Status.Should().Be(VehicleStatus.Available);
    }

    [Fact]
    public void UpdateDailyRate_WithDifferentRate_UpdatesRateAndRaisesDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var oldRate = vehicle.DailyRate;
        var newRate = Money.FromGross(75.00m, 0.19m, Currency.Of("EUR"));

        // Act
        vehicle.UpdateDailyRate(newRate);

        // Assert
        vehicle.DailyRate.Should().Be(newRate);
        vehicle.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void UpdateDailyRate_WithSameRate_DoesNotRaiseDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var sameRate = vehicle.DailyRate;

        // Act
        vehicle.UpdateDailyRate(sameRate);

        // Assert
        vehicle.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MoveToLocation_WithDifferentLocation_UpdatesLocationAndRaisesDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var newLocation = Location.Of("MUC-FLG", "Munich Airport");

        // Act
        vehicle.MoveToLocation(newLocation);

        // Assert
        vehicle.CurrentLocation.Should().Be(newLocation);
        vehicle.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void MoveToLocation_WhenVehicleIsRented_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.MarkAsRented();
        var newLocation = Location.Of("MUC-FLG", "Munich Airport");

        // Act & Assert
        var act = () => vehicle.MoveToLocation(newLocation);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot move a rented vehicle");
    }

    [Fact]
    public void ChangeStatus_WithDifferentStatus_UpdatesStatusAndRaisesDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.Maintenance);
        vehicle.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void ChangeStatus_WithSameStatus_DoesNotRaiseDomainEvent()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        var currentStatus = vehicle.Status;

        // Act
        vehicle.ChangeStatus(currentStatus);

        // Assert
        vehicle.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MarkAsAvailable_WhenNotAvailable_ChangesStatusToAvailable()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.ChangeStatus(VehicleStatus.Maintenance);

        // Act
        vehicle.MarkAsAvailable();

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.Available);
    }

    [Fact]
    public void MarkAsAvailable_WhenAlreadyAvailable_DoesNothing()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.ClearDomainEvents();

        // Act
        vehicle.MarkAsAvailable();

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.Available);
        vehicle.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MarkAsRented_ChangesStatusToRented()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle.MarkAsRented();

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.Rented);
        vehicle.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void MarkAsAvailable_AfterRented_ChangesStatusToAvailable()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.MarkAsRented();
        vehicle.ClearDomainEvents();

        // Act
        vehicle.MarkAsAvailable();

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.Available);
        vehicle.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenAvailable_ChangesStatusToMaintenance()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle.MarkAsUnderMaintenance();

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.Maintenance);
    }

    [Fact]
    public void MarkAsUnderMaintenance_WhenRented_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.MarkAsRented();

        // Act & Assert
        var act = () => vehicle.MarkAsUnderMaintenance();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot put rented vehicle under maintenance");
    }

    [Fact]
    public void ChangeStatus_ToOutOfService_ChangesStatus()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle.ChangeStatus(VehicleStatus.OutOfService);

        // Assert
        vehicle.Status.Should().Be(VehicleStatus.OutOfService);
        vehicle.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void SetLicensePlate_WithValidPlate_UpdatesLicensePlate()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle.SetLicensePlate("B-AB-9999");

        // Assert
        vehicle.LicensePlate.Should().Be("B-AB-9999");
    }

    [Fact]
    public void SetLicensePlate_WithWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act & Assert
        var act = () => vehicle.SetLicensePlate("  ");
        act.Should().Throw<ArgumentException>()
            .WithMessage("License plate cannot be empty*");
    }

    [Fact]
    public void SetLicensePlate_WithEmpty_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act & Assert
        var act = () => vehicle.SetLicensePlate("");
        act.Should().Throw<ArgumentException>()
            .WithMessage("License plate cannot be empty*");
    }

    [Fact]
    public void SetLicensePlate_NormalizesToUpperCase()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        vehicle.SetLicensePlate("b-xy-1234");

        // Assert
        vehicle.LicensePlate.Should().Be("B-XY-1234");
    }

    [Fact]
    public void IsAvailableForRental_WhenAvailable_ReturnsTrue()
    {
        // Arrange
        var vehicle = CreateTestVehicle();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.Should().BeTrue();
    }

    [Fact]
    public void IsAvailableForRental_WhenRented_ReturnsFalse()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.MarkAsRented();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.Should().BeFalse();
    }

    [Fact]
    public void IsAvailableForRental_WhenInMaintenance_ReturnsFalse()
    {
        // Arrange
        var vehicle = CreateTestVehicle();
        vehicle.MarkAsUnderMaintenance();

        // Act
        var isAvailable = vehicle.IsAvailableForRental();

        // Assert
        isAvailable.Should().BeFalse();
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
        vehicle.SetLicensePlate("B-XY-1234");
        vehicle.ClearDomainEvents();
        return vehicle;
    }
}
