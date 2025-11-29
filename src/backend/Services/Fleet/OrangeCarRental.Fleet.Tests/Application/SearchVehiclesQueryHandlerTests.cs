using Moq;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Application;

public class SearchVehiclesQueryHandlerTests
{
    private readonly SearchVehiclesQueryHandler handler;
    private readonly Mock<IVehicleRepository> repositoryMock = new();

    public SearchVehiclesQueryHandlerTests()
    {
        handler = new SearchVehiclesQueryHandler(repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithNoFilters_ReturnsAllVehicles()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            1,
            20);

        var vehicles = CreateTestVehicles();
        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = vehicles.Count,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

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
            null,
            null,
            "BER-HBF",
            null,
            null,
            null,
            null,
            null,
            1,
            20);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SearchAsync(
                It.Is<VehicleSearchParameters>(p =>
                    p.LocationCode.HasValue && p.LocationCode.Value == Locations.BerlinHauptbahnhof &&
                    p.Status == VehicleStatus.Available),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithFuelTypeFilter_ParsesAndPassesEnumCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            null,
            null,
            null,
            null,
            null,
            "Electric",
            null,
            null,
            1,
            20);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SearchAsync(
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
        var query = new SearchVehiclesQuery(
            null,
            null,
            null,
            null,
            null,
            null,
            "Automatic",
            null,
            1,
            20);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SearchAsync(
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
        var query = new SearchVehiclesQuery(
            null,
            null,
            "BER-HBF",
            VehicleCategory.Mittelklasse.Code,
            5,
            "Petrol",
            "Manual",
            75.00m,
            2,
            10
        );

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 2,
            PageSize = 10
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SearchAsync(
                It.Is<VehicleSearchParameters>(p =>
                    p.LocationCode.HasValue && p.LocationCode.Value == Locations.BerlinHauptbahnhof &&
                    p.Category.HasValue && p.Category.Value.Code == VehicleCategory.Mittelklasse.Code &&
                    p.MinSeats.HasValue && p.MinSeats.Value.Value == 5 &&
                    p.FuelType == FuelType.Petrol &&
                    p.TransmissionType == TransmissionType.Manual &&
                    p.MaxDailyRate.HasValue && p.MaxDailyRate.Value.GrossAmount == 75.00m &&
                    p.Status == VehicleStatus.Available &&
                    p.Paging.PageNumber == 2 &&
                    p.Paging.PageSize == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_MapsVehiclesToDtosCorrectly()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            1,
            20);
        var vehicles = CreateTestVehicles();
        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = vehicles.Count,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Vehicles.Count.ShouldBe(3);
        var firstDto = result.Vehicles.First();
        var firstVehicle = vehicles.First();

        firstDto.Id.ShouldBe(firstVehicle.Id.Value);
        firstDto.Name.ShouldBe(firstVehicle.Name.Value);
        firstDto.CategoryCode.ShouldBe(firstVehicle.Category.Code);
        firstDto.LocationCode.ShouldBe(firstVehicle.CurrentLocationCode.Value);
        firstDto.Seats.ShouldBe(firstVehicle.Seats.Value);
        firstDto.FuelType.ShouldBe(firstVehicle.FuelType.ToString());
        firstDto.TransmissionType.ShouldBe(firstVehicle.TransmissionType.ToString());
        firstDto.DailyRateGross.ShouldBe(firstVehicle.DailyRate.GrossAmount);
    }

    [Fact]
    public async Task HandleAsync_WithDefaultPagination_UsesDefaults()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null); // No pagination specified

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SearchAsync(
                It.Is<VehicleSearchParameters>(p =>
                    p.Paging.PageNumber == 1 &&
                    p.Paging.PageSize == 20),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_AlwaysFiltersForAvailableVehicles()
    {
        // Arrange
        var query = new SearchVehiclesQuery(
            null,
            null,
            "BER-HBF",
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 20
        };

        repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SearchAsync(
                It.Is<VehicleSearchParameters>(p => p.Status == VehicleStatus.Available),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private List<Vehicle> CreateTestVehicles()
    {
        return
        [
            CreateVehicle("VW Polo", VehicleCategory.Kleinwagen.Code, Locations.BerlinHauptbahnhof, FuelType.Petrol, 4, 35.00m),
            CreateVehicle("VW Golf", VehicleCategory.Mittelklasse.Code, Locations.BerlinHauptbahnhof, FuelType.Petrol, 5, 55.00m),
            CreateVehicle("Tesla Model 3", VehicleCategory.Luxus.Code,Locations.MunichAirport, FuelType.Electric, 5, 95.00m)
        ];
    }

    private Vehicle CreateVehicle(
        string name,
        string categoryCode,
        LocationCode locationCode,
        FuelType fuelType,
        int seats,
        decimal dailyRateGross)
    {
        var category = VehicleCategory.From(categoryCode);
        var currency = Currency.From("EUR");
        var dailyRate = Money.FromGross(dailyRateGross, 0.19m, currency);

        var vehicle = Vehicle.From(
            VehicleName.From(name),
            category,
            locationCode,
            dailyRate,
            SeatingCapacity.From(seats),
            fuelType,
            TransmissionType.Manual
        );
        vehicle.SetLicensePlate($"B-TEST-{Guid.NewGuid().ToString()[..6]}");
        return vehicle;
    }
}
