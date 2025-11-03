using Moq;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Application;

public class SearchVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _repositoryMock;
    private readonly SearchVehiclesQueryHandler _handler;

    public SearchVehiclesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _handler = new SearchVehiclesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithNoFilters_ReturnsAllVehicles()
    {
        // Arrange
        var query = new SearchVehiclesQuery
        {
            PageNumber = 1,
            PageSize = 20
        };

        var vehicles = CreateTestVehicles();
        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = vehicles.Count,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Vehicles.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_WithLocationFilter_PassesCorrectParameters()
    {
        // Arrange
        var query = new SearchVehiclesQuery
        {
            LocationCode = "BER-HBF",
            PageNumber = 1,
            PageSize = 20
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync(
            It.Is<VehicleSearchParameters>(p =>
                p.LocationCode == "BER-HBF" &&
                p.Status == VehicleStatus.Available),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithFuelTypeFilter_ParsesAndPassesEnumCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery
        {
            FuelType = "Electric",
            PageNumber = 1,
            PageSize = 20
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync(
            It.Is<VehicleSearchParameters>(p =>
                p.FuelType == FuelType.Electric &&
                p.Status == VehicleStatus.Available),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithTransmissionFilter_ParsesAndPassesEnumCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery
        {
            TransmissionType = "Automatic",
            PageNumber = 1,
            PageSize = 20
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync(
            It.Is<VehicleSearchParameters>(p =>
                p.TransmissionType == TransmissionType.Automatic &&
                p.Status == VehicleStatus.Available),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithAllFilters_PassesAllParametersCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery
        {
            LocationCode = "BER-HBF",
            CategoryCode = "MITTEL",
            MinSeats = 5,
            FuelType = "Petrol",
            TransmissionType = "Manual",
            MaxDailyRateGross = 75.00m,
            PageNumber = 2,
            PageSize = 10
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 2,
            PageSize = 10
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync(
            It.Is<VehicleSearchParameters>(p =>
                p.LocationCode == "BER-HBF" &&
                p.CategoryCode == "MITTEL" &&
                p.MinSeats == 5 &&
                p.FuelType == FuelType.Petrol &&
                p.TransmissionType == TransmissionType.Manual &&
                p.MaxDailyRateGross == 75.00m &&
                p.Status == VehicleStatus.Available &&
                p.PageNumber == 2 &&
                p.PageSize == 10),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_MapsVehiclesToDtosCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery { PageNumber = 1, PageSize = 20 };
        var vehicles = CreateTestVehicles();
        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = vehicles.Count,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Vehicles.Should().HaveCount(3);
        var firstDto = result.Vehicles.First();
        var firstVehicle = vehicles.First();

        firstDto.Id.Should().Be(firstVehicle.Id.Value);
        firstDto.Name.Should().Be(firstVehicle.Name.Value);
        firstDto.CategoryCode.Should().Be(firstVehicle.Category.Code);
        firstDto.LocationCode.Should().Be(firstVehicle.CurrentLocation.Code.Value);
        firstDto.Seats.Should().Be(firstVehicle.Seats.Value);
        firstDto.FuelType.Should().Be(firstVehicle.FuelType.ToString());
        firstDto.TransmissionType.Should().Be(firstVehicle.TransmissionType.ToString());
        firstDto.DailyRateGross.Should().Be(firstVehicle.DailyRate.GrossAmount);
    }

    [Fact]
    public async Task HandleAsync_WithDefaultPagination_UsesDefaults()
    {
        // Arrange
        var query = new SearchVehiclesQuery(); // No pagination specified

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync(
            It.Is<VehicleSearchParameters>(p =>
                p.PageNumber == 1 &&
                p.PageSize == 20),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_AlwaysFiltersForAvailableVehicles()
    {
        // Arrange
        var query = new SearchVehiclesQuery
        {
            LocationCode = "BER-HBF"
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync(
            It.Is<VehicleSearchParameters>(p => p.Status == VehicleStatus.Available),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private List<Vehicle> CreateTestVehicles()
    {
        return new List<Vehicle>
        {
            CreateVehicle("VW Polo", "KLEIN", "BER-HBF", FuelType.Petrol, 4, 35.00m),
            CreateVehicle("VW Golf", "MITTEL", "BER-HBF", FuelType.Petrol, 5, 55.00m),
            CreateVehicle("Tesla Model 3", "LUXUS", "MUC-FLG", FuelType.Electric, 5, 95.00m)
        };
    }

    private Vehicle CreateVehicle(
        string name,
        string categoryCode,
        string locationCode,
        FuelType fuelType,
        int seats,
        decimal dailyRateGross)
    {
        var category = VehicleCategory.FromCode(categoryCode);
        var location = Location.Of(locationCode, "Test City");
        var currency = Currency.Of("EUR");
        var dailyRate = Money.FromGross(dailyRateGross, 0.19m, currency);

        var vehicle = Vehicle.From(
            VehicleName.Of(name),
            category,
            location,
            dailyRate,
            SeatingCapacity.Of(seats),
            fuelType,
            TransmissionType.Manual
        );
        vehicle.SetLicensePlate($"B-TEST-{Guid.NewGuid().ToString()[..6]}");
        return vehicle;
    }
}
