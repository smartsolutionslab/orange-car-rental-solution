using Moq;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Application;

public class SearchVehiclesQueryHandlerTests
{
    private readonly SearchVehiclesQueryHandler _handler;
    private readonly Mock<IVehicleRepository> _repositoryMock = new();

    public SearchVehiclesQueryHandlerTests()
    {
        _handler = new SearchVehiclesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithNoFilters_ReturnsAllVehicles()
    {
        // Arrange
        var query = new SearchVehiclesQuery(null, null, null, null, null, null,
            null, null, 1, 20);

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
        result.ShouldNotBeNull();
        result.Vehicles.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(20);
        result.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task HandleAsync_WithLocationFilter_PassesCorrectParameters()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            null, null, "BER-HBF", null, null, null, null, null,
            1, 20);

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
                    p.LocationCode.HasValue && p.LocationCode.Value.Value == "BER-HBF" &&
                    p.Status == VehicleStatus.Available),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithFuelTypeFilter_ParsesAndPassesEnumCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery(null, null, null, null, null,"Electric",
            null,null, 1,20);

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
        var query = new SearchVehiclesQuery(null, null, null, null, null, null, "Automatic", null, 1, 20);

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
        var query = new SearchVehiclesQuery(null, null, "BER-HBF", "MITTEL", 5,
            "Petrol",
            "Manual",
            75.00m,
            2,
            10
        );

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
                    p.LocationCode.HasValue && p.LocationCode.Value.Value == "BER-HBF" &&
                    p.Category.HasValue && p.Category.Value.Code == "MITTEL" &&
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
        var query = new SearchVehiclesQuery(null, null, null, null, null, null, null,null, 1, 20);
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
        result.Vehicles.Count.ShouldBe(3);
        var firstDto = result.Vehicles.First();
        var firstVehicle = vehicles.First();

        firstDto.Id.ShouldBe(firstVehicle.Id.Value);
        firstDto.Name.ShouldBe(firstVehicle.Name.Value);
        firstDto.CategoryCode.ShouldBe(firstVehicle.Category.Code);
        firstDto.LocationCode.ShouldBe(firstVehicle.CurrentLocation.Code.Value);
        firstDto.Seats.ShouldBe(firstVehicle.Seats.Value);
        firstDto.FuelType.ShouldBe(firstVehicle.FuelType.ToString());
        firstDto.TransmissionType.ShouldBe(firstVehicle.TransmissionType.ToString());
        firstDto.DailyRateGross.ShouldBe(firstVehicle.DailyRate.GrossAmount);
    }

    [Fact]
    public async Task HandleAsync_WithDefaultPagination_UsesDefaults()
    {
        // Arrange
        var query = new SearchVehiclesQuery(null, null, null, null, null, null, null, null, null,null); // No pagination specified

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
        var query = new SearchVehiclesQuery(null, null, "BER-HBF", null, null, null, null, null, null, null);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = [],
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
        return
        [
            CreateVehicle("VW Polo", "KLEIN", "BER-HBF", FuelType.Petrol, 4, 35.00m),
            CreateVehicle("VW Golf", "MITTEL", "BER-HBF", FuelType.Petrol, 5, 55.00m),
            CreateVehicle("Tesla Model 3", "LUXUS", "MUC-FLG", FuelType.Electric, 5, 95.00m)
        ];
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
