using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Application.Commands;

public class AddVehicleToFleetCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly AddVehicleToFleetCommandHandler _handler;

    public AddVehicleToFleetCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _handler = new AddVehicleToFleetCommandHandler(_vehicleRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldAddVehicleToFleet()
    {
        // Arrange
        var command = CreateValidCommand();

        Vehicle? addedVehicle = null;
        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Callback<Vehicle, CancellationToken>((vehicle, _) => addedVehicle = vehicle)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.VehicleId.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(command.Name.Value);
        result.CategoryCode.ShouldBe(command.Category.Code);
        result.Status.ShouldBe("Available");
        result.LocationCode.ShouldBe(command.CurrentLocation.Code.Value);
        result.NetAmount.ShouldBe(command.DailyRate.NetAmount);
        result.VatAmount.ShouldBe(command.DailyRate.VatAmount);
        result.GrossAmount.ShouldBe(command.DailyRate.GrossAmount);

        addedVehicle.ShouldNotBeNull();
        addedVehicle.Name.ShouldBe(command.Name);
        addedVehicle.Category.ShouldBe(command.Category);
        addedVehicle.CurrentLocation.ShouldBe(command.CurrentLocation);
        addedVehicle.DailyRate.ShouldBe(command.DailyRate);
        addedVehicle.Seats.ShouldBe(command.Seats);
        addedVehicle.FuelType.ShouldBe(command.FuelType);
        addedVehicle.TransmissionType.ShouldBe(command.TransmissionType);

        _vehicleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
        _vehicleRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithOptionalDetails_ShouldSetDetailsCorrectly()
    {
        // Arrange
        var manufacturer = Manufacturer.Of("BMW");
        var model = VehicleModel.Of("X5 xDrive40i");
        var year = ManufacturingYear.Of(2024);
        var imageUrl = "https://example.com/bmw-x5.jpg";
        var licensePlate = "B-AB 1234";

        var command = CreateValidCommand() with
        {
            Manufacturer = manufacturer,
            Model = model,
            Year = year,
            ImageUrl = imageUrl,
            LicensePlate = licensePlate
        };

        Vehicle? addedVehicle = null;
        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Callback<Vehicle, CancellationToken>((vehicle, _) => addedVehicle = vehicle)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        addedVehicle.ShouldNotBeNull();
        addedVehicle.Manufacturer.ShouldBe(manufacturer);
        addedVehicle.Model.ShouldBe(model);
        addedVehicle.Year.ShouldBe(year);
        addedVehicle.ImageUrl.ShouldBe(imageUrl);
        addedVehicle.LicensePlate.ShouldBe(licensePlate);
    }

    [Fact]
    public async Task HandleAsync_WithoutOptionalDetails_ShouldCreateVehicleWithoutDetails()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Manufacturer = null,
            Model = null,
            Year = null,
            ImageUrl = null,
            LicensePlate = null
        };

        Vehicle? addedVehicle = null;
        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Callback<Vehicle, CancellationToken>((vehicle, _) => addedVehicle = vehicle)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        addedVehicle.ShouldNotBeNull();
        addedVehicle.Manufacturer.ShouldBeNull();
        addedVehicle.Model.ShouldBeNull();
        addedVehicle.Year.ShouldBeNull();
        addedVehicle.ImageUrl.ShouldBeNull();
        addedVehicle.LicensePlate.ShouldBeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _vehicleRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = CreateValidCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithDifferentVehicleCategories_ShouldCreateCorrectly()
    {
        // Arrange
        var categories = new[]
        {
            VehicleCategory.Kleinwagen,
            VehicleCategory.Kompaktklasse,
            VehicleCategory.Mittelklasse,
            VehicleCategory.Oberklasse,
            VehicleCategory.SUV,
            VehicleCategory.Kombi,
            VehicleCategory.Transporter,
            VehicleCategory.Luxus
        };

        foreach (var category in categories)
        {
            var command = CreateValidCommand() with { Category = category };

            Vehicle? addedVehicle = null;
            _vehicleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((vehicle, _) => addedVehicle = vehicle)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            addedVehicle.ShouldNotBeNull();
            addedVehicle.Category.ShouldBe(category);
            result.CategoryCode.ShouldBe(category.Code);
        }
    }

    [Fact]
    public async Task HandleAsync_WithDifferentFuelTypes_ShouldCreateCorrectly()
    {
        // Arrange
        var fuelTypes = new[]
        {
            FuelType.Petrol,
            FuelType.Diesel,
            FuelType.Electric,
            FuelType.Hybrid,
            FuelType.PlugInHybrid,
            FuelType.Hydrogen
        };

        foreach (var fuelType in fuelTypes)
        {
            var command = CreateValidCommand() with { FuelType = fuelType };

            Vehicle? addedVehicle = null;
            _vehicleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((vehicle, _) => addedVehicle = vehicle)
                .Returns(Task.CompletedTask);

            // Act
            await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            addedVehicle.ShouldNotBeNull();
            addedVehicle.FuelType.ShouldBe(fuelType);
        }
    }

    private static AddVehicleToFleetCommand CreateValidCommand()
    {
        return new AddVehicleToFleetCommand(
            VehicleName.Of("BMW X5"),
            VehicleCategory.SUV,
            Location.BerlinHauptbahnhof,
            Money.Of(89.99m, Currency.EUR),
            SeatingCapacity.Of(5),
            FuelType.Diesel,
            TransmissionType.Automatic,
            null, // LicensePlate
            null, // Manufacturer
            null, // Model
            null, // Year
            null  // ImageUrl
        );
    }
}
