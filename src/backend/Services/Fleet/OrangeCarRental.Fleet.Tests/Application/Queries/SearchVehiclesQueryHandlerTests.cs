using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Application.Queries;

public class SearchVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> vehicleRepositoryMock = new();
    private readonly SearchVehiclesQueryHandler handler;

    public SearchVehiclesQueryHandlerTests()
    {
        handler = new SearchVehiclesQueryHandler(vehicleRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSearchResults()
    {
        // Arrange
        var query = CreateQuery(
            period: SearchPeriod.Of(
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))),
            locationCode: LocationCode.From("BER-HBF"),
            category: VehicleCategory.SUV,
            minSeats: SeatingCapacity.From(5),
            fuelType: FuelType.Diesel,
            transmissionType: TransmissionType.Automatic,
            maxDailyRate: Money.EuroGross(100.00m));

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("BMW X5", VehicleCategory.SUV, Locations.BerlinHauptbahnhof),
            CreateTestVehicle("Audi Q7", VehicleCategory.SUV, Locations.BerlinHauptbahnhof)
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };

        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.TotalCount.ShouldBe(2);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalPages.ShouldBe(1);

        vehicleRepositoryMock.Verify(
            x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyResults_ShouldReturnEmptyList()
    {
        // Arrange
        var query = CreateQuery();

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = new List<Vehicle>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
        result.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ShouldPassCorrectParameters()
    {
        // Arrange
        var query = CreateQuery(pageNumber: 2, pageSize: 20);

        VehicleSearchParameters? capturedParameters = null;
        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .Callback<VehicleSearchParameters, CancellationToken>((param, _) => capturedParameters = param)
            .ReturnsAsync(new PagedResult<Vehicle>
            {
                Items = new List<Vehicle>(),
                TotalCount = 0,
                PageNumber = 2,
                PageSize = 20
            });

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        capturedParameters.ShouldNotBeNull();
        capturedParameters.Paging.PageNumber.ShouldBe(2);
        capturedParameters.Paging.PageSize.ShouldBe(20);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var query = CreateQuery();

        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            handler.HandleAsync(query, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithLocationFilter_ShouldFilterByLocation()
    {
        // Arrange
        var query = CreateQuery(locationCode: LocationCode.From("BER-HBF"));

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("BMW X5", VehicleCategory.SUV, Locations.BerlinHauptbahnhof)
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(1);
        result.Items[0].LocationCode.ShouldBe("BER-HBF");
    }

    [Fact]
    public async Task HandleAsync_WithCategoryFilter_ShouldFilterByCategory()
    {
        // Arrange
        var query = CreateQuery(category: VehicleCategory.SUV);

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("BMW X5", VehicleCategory.SUV, Locations.BerlinHauptbahnhof)
        };

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(1);
        result.Items[0].CategoryCode.ShouldBe("SUV");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleFilters_ShouldCombineFilters()
    {
        // Arrange
        var query = CreateQuery(
            period: SearchPeriod.Of(
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))),
            locationCode: Locations.BerlinHauptbahnhof,
            category: VehicleCategory.SUV,
            minSeats: SeatingCapacity.From(5),
            fuelType: FuelType.Diesel,
            transmissionType: TransmissionType.Automatic,
            maxDailyRate: Money.EuroGross(100.00m));

        VehicleSearchParameters? capturedParameters = null;
        vehicleRepositoryMock
            .Setup(x => x.SearchAsync(It.IsAny<VehicleSearchParameters>(), It.IsAny<CancellationToken>()))
            .Callback<VehicleSearchParameters, CancellationToken>((param, _) => capturedParameters = param)
            .ReturnsAsync(new PagedResult<Vehicle>
            {
                Items = new List<Vehicle>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            });

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        capturedParameters.ShouldNotBeNull();
        capturedParameters.MinSeats!.Value.Value.ShouldBe(5);
        capturedParameters.MaxDailyRate!.Value.GrossAmount.ShouldBe(100.00m);
    }

    private static SearchVehiclesQuery CreateQuery(
        SearchPeriod? period = null,
        LocationCode? locationCode = null,
        VehicleCategory? category = null,
        SeatingCapacity? minSeats = null,
        FuelType? fuelType = null,
        TransmissionType? transmissionType = null,
        Money? maxDailyRate = null,
        VehicleStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        return new SearchVehiclesQuery(
            period,
            locationCode,
            category,
            minSeats,
            fuelType,
            transmissionType,
            maxDailyRate,
            status,
            PagingInfo.Create(pageNumber, pageSize),
            SortingInfo.Create());
    }

    private static Vehicle CreateTestVehicle(string name, VehicleCategory category, LocationCode location)
    {
        return VehicleBuilder.Default()
            .WithName(name)
            .WithCategory(category)
            .AtLocation(location)
            .Build();
    }
}
