using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Application.Queries;

public class SearchVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly SearchVehiclesQueryHandler _handler;

    public SearchVehiclesQueryHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _handler = new SearchVehiclesQueryHandler(_vehicleRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSearchResults()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: DateTime.UtcNow.AddDays(1),
            ReturnDate: DateTime.UtcNow.AddDays(3),
            LocationCode: "BER-HBF",
            CategoryCode: "SUV",
            MinSeats: 5,
            FuelType: "Diesel",
            TransmissionType: "Automatic",
            MaxDailyRateGross: 100.00m,
            PageNumber: 1,
            PageSize: 10
        );

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("BMW X5", VehicleCategory.SUV, Location.BerlinHauptbahnhof),
            CreateTestVehicle("Audi Q7", VehicleCategory.SUV, Location.BerlinHauptbahnhof)
        };

        var pagedResult = new PagedResult<Vehicle>(vehicles, 2, 1, 10);

        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Vehicles.Count.ShouldBe(2);
        result.TotalCount.ShouldBe(2);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalPages.ShouldBe(1);

        _vehicleRepositoryMock.Verify(
            x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyResults_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: null,
            ReturnDate: null,
            LocationCode: null,
            CategoryCode: null,
            MinSeats: null,
            FuelType: null,
            TransmissionType: null,
            MaxDailyRateGross: null,
            PageNumber: 1,
            PageSize: 10
        );

        var pagedResult = new PagedResult<Vehicle>(new List<Vehicle>(), 0, 1, 10);

        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Vehicles.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
        result.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ShouldPassCorrectParameters()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: null,
            ReturnDate: null,
            LocationCode: null,
            CategoryCode: null,
            MinSeats: null,
            FuelType: null,
            TransmissionType: null,
            MaxDailyRateGross: null,
            PageNumber: 2,
            PageSize: 20
        );

        VehicleSearchParameters? capturedParameters = null;
        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .Callback<VehicleSearchParameters, CancellationToken>((param, _) => capturedParameters = param)
            .ReturnsAsync(new PagedResult<Vehicle>(new List<Vehicle>(), 0, 2, 20));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        capturedParameters.ShouldNotBeNull();
        capturedParameters.PageNumber.ShouldBe(2);
        capturedParameters.PageSize.ShouldBe(20);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: null,
            ReturnDate: null,
            LocationCode: null,
            CategoryCode: null,
            MinSeats: null,
            FuelType: null,
            TransmissionType: null,
            MaxDailyRateGross: null,
            PageNumber: 1,
            PageSize: 10
        );

        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(query, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithLocationFilter_ShouldFilterByLocation()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: null,
            ReturnDate: null,
            LocationCode: "BER-HBF",
            CategoryCode: null,
            MinSeats: null,
            FuelType: null,
            TransmissionType: null,
            MaxDailyRateGross: null,
            PageNumber: 1,
            PageSize: 10
        );

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("BMW X5", VehicleCategory.SUV, Location.BerlinHauptbahnhof)
        };

        var pagedResult = new PagedResult<Vehicle>(vehicles, 1, 1, 10);

        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Vehicles.Count.ShouldBe(1);
        result.Vehicles[0].LocationCode.ShouldBe("BER-HBF");
    }

    [Fact]
    public async Task HandleAsync_WithCategoryFilter_ShouldFilterByCategory()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: null,
            ReturnDate: null,
            LocationCode: null,
            CategoryCode: "SUV",
            MinSeats: null,
            FuelType: null,
            TransmissionType: null,
            MaxDailyRateGross: null,
            PageNumber: 1,
            PageSize: 10
        );

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("BMW X5", VehicleCategory.SUV, Location.BerlinHauptbahnhof)
        };

        var pagedResult = new PagedResult<Vehicle>(vehicles, 1, 1, 10);

        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Vehicles.Count.ShouldBe(1);
        result.Vehicles[0].CategoryCode.ShouldBe("SUV");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleFilters_ShouldCombineFilters()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            PickupDate: DateTime.UtcNow.AddDays(1),
            ReturnDate: DateTime.UtcNow.AddDays(3),
            LocationCode: "BER-HBF",
            CategoryCode: "SUV",
            MinSeats: 5,
            FuelType: "Diesel",
            TransmissionType: "Automatic",
            MaxDailyRateGross: 100.00m,
            PageNumber: 1,
            PageSize: 10
        );

        VehicleSearchParameters? capturedParameters = null;
        _vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .Callback<VehicleSearchParameters, CancellationToken>((param, _) => capturedParameters = param)
            .ReturnsAsync(new PagedResult<Vehicle>(new List<Vehicle>(), 0, 1, 10));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        capturedParameters.ShouldNotBeNull();
        capturedParameters.MinSeats.ShouldBe(5);
        capturedParameters.MaxDailyRateGross.ShouldBe(100.00m);
    }

    private static Vehicle CreateTestVehicle(string name, VehicleCategory category, Location location)
    {
        return Vehicle.From(
            VehicleName.Of(name),
            category,
            location,
            Money.Euro(89.99m),
            SeatingCapacity.Of(5),
            FuelType.Diesel,
            TransmissionType.Automatic);
    }
}
