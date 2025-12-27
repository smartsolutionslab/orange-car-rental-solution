using Microsoft.EntityFrameworkCore;
using Moq;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;
using Testcontainers.MsSql;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Infrastructure;

public class VehicleRepositoryTests : IAsyncLifetime
{
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private FleetDbContext context = null!;
    private VehicleRepository repository = null!;
    private Mock<IReservationService> reservationServiceMock = null!;

    public async Task InitializeAsync()
    {
        // Start the SQL Server container
        await msSqlContainer.StartAsync();

        // Create DbContext with SQL Server connection
        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseSqlServer(msSqlContainer.GetConnectionString())
            .Options;

        context = new FleetDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Create mock for IReservationService
        reservationServiceMock = new Mock<IReservationService>();

        // Default mock behavior: return empty list (no vehicles booked)
        reservationServiceMock
            .Setup(x => x.GetBookedVehicleIdsAsync(It.IsAny<SearchPeriod>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<VehicleIdentifier>());

        repository = new VehicleRepository(context, reservationServiceMock.Object);
    }

    public async Task DisposeAsync()
    {
        await context.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAllAvailableVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(5); // All 5 test vehicles
        result.TotalCount.ShouldBe(5);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(20);
        result.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task SearchAsync_WithLocationFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            LocationCode.From(Locations.BerlinHauptbahnhof),
            default, default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(3);
        result.Items.ShouldAllBe(v => v.CurrentLocationCode.Value == Locations.BerlinHauptbahnhof);
        result.TotalCount.ShouldBe(3);
    }

    [Fact]
    public async Task SearchAsync_WithCategoryFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default,
            VehicleCategory.Kleinwagen,
            default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(v => v.Category.Code == VehicleCategory.Kleinwagen.Code);
    }

    [Fact]
    public async Task SearchAsync_WithFuelTypeFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default,
            FuelType.Electric,
            default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(1);
        result.Items.First().FuelType.ShouldBe(FuelType.Electric);
    }

    [Fact]
    public async Task SearchAsync_WithMinSeatsFilter_ReturnsVehiclesWithEnoughSeats()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default,
            SeatingCapacity.From(5),
            default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        // Test data has: VW Golf (5), Tesla (5), Ford Focus (5), BMW X5 (7) = 4 vehicles with >= 5 seats
        result.Items.Count.ShouldBe(4);
        result.Items.ShouldAllBe(v => v.Seats.Value >= 5);
    }

    [Fact]
    public async Task SearchAsync_WithMaxPriceFilter_ReturnsVehiclesWithinBudget()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default,
            Money.EuroGross(50.00m),
            default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(v => v.DailyRate.GrossAmount <= 50.00m);
    }

    [Fact]
    public async Task SearchAsync_WithMultipleFilters_ReturnsMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            LocationCode.From(Locations.BerlinHauptbahnhof),
            default,
            SeatingCapacity.From(4),
            FuelType.Petrol,
            default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(2);
        foreach (var v in result.Items)
        {
            v.CurrentLocationCode.Value.ShouldBe(Locations.BerlinHauptbahnhof.Value);
            v.FuelType.ShouldBe(FuelType.Petrol);
            v.Seats.Value.ShouldBeGreaterThanOrEqualTo(4);
        }
    }

    [Fact]
    public async Task SearchAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default, default, default,
            PagingInfo.Create(2, 2),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(2);
        result.PageNumber.ShouldBe(2);
        result.PageSize.ShouldBe(2);
        result.TotalCount.ShouldBe(5);
        result.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task SearchAsync_WithStatusFilter_ReturnsOnlyAvailableVehicles()
    {
        // Arrange
        await SeedTestDataAsync();

        // Mark one vehicle as rented
        var vehicle = await context.Vehicles.FirstAsync();
        context.Entry(vehicle).State = EntityState.Detached;
        vehicle = vehicle.ChangeStatus(VehicleStatus.Rented);
        context.Vehicles.Update(vehicle);
        await context.SaveChangesAsync();

        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default,
            VehicleStatus.Available,
            default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(4);
        result.Items.ShouldAllBe(v => v.Status == VehicleStatus.Available);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsVehicle()
    {
        // Arrange
        var vehicle = CreateTestVehicle("VW Polo", VehicleCategory.Kleinwagen.Code, Locations.BerlinHauptbahnhof, FuelType.Petrol, 4, 35.00m);
        await context.Vehicles.AddAsync(vehicle);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(vehicle.Id, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(vehicle.Id);
        result.Name.Value.ShouldBe("VW Polo");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistingId = VehicleIdentifier.New();

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await repository.GetByIdAsync(nonExistingId, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WithDefaultSorting_ReturnsSortedByNameAscending()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.None);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(5);
        // Default sorting is by name ascending: BMW X5, Ford Focus, Tesla Model 3, VW Golf, VW Polo
        result.Items[0].Name.Value.ShouldBe("BMW X5");
        result.Items[1].Name.Value.ShouldBe("Ford Focus");
        result.Items[2].Name.Value.ShouldBe("Tesla Model 3");
        result.Items[3].Name.Value.ShouldBe("VW Golf");
        result.Items[4].Name.Value.ShouldBe("VW Polo");
    }

    [Fact]
    public async Task SearchAsync_WithNameDescending_ReturnsSortedByNameDescending()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.DescendingBy("name"));

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(5);
        // Name descending: VW Polo, VW Golf, Tesla Model 3, Ford Focus, BMW X5
        result.Items[0].Name.Value.ShouldBe("VW Polo");
        result.Items[1].Name.Value.ShouldBe("VW Golf");
        result.Items[4].Name.Value.ShouldBe("BMW X5");
    }

    [Fact]
    public async Task SearchAsync_WithPriceAscending_ReturnsSortedByPriceAscending()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.AscendingBy("dailyrate"));

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(5);
        // Price ascending: VW Polo (35), Ford Focus (45), VW Golf (55), Tesla (95), BMW X5 (120)
        result.Items[0].Name.Value.ShouldBe("VW Polo");
        result.Items[1].Name.Value.ShouldBe("Ford Focus");
        result.Items[2].Name.Value.ShouldBe("VW Golf");
        result.Items[3].Name.Value.ShouldBe("Tesla Model 3");
        result.Items[4].Name.Value.ShouldBe("BMW X5");
    }

    [Fact]
    public async Task SearchAsync_WithSeatsDescending_ReturnsSortedBySeatsDescending()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(
            default, default, default, default, default, default, default, default,
            PagingInfo.Create(1, 20),
            SortingInfo.DescendingBy("seats"));

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(5);
        // Seats descending: BMW X5 (7), then any with 5 seats, VW Polo (4)
        result.Items[0].Name.Value.ShouldBe("BMW X5");
        result.Items[0].Seats.Value.ShouldBe(7);
        result.Items[4].Name.Value.ShouldBe("VW Polo");
        result.Items[4].Seats.Value.ShouldBe(4);
    }

    private async Task SeedTestDataAsync()
    {
        var vehicles = new[]
        {
            CreateTestVehicle("VW Polo", VehicleCategory.Kleinwagen.Code, Locations.BerlinHauptbahnhof, FuelType.Petrol, 4, 35.00m),
            CreateTestVehicle("VW Golf", VehicleCategory.Mittelklasse.Code, Locations.BerlinHauptbahnhof, FuelType.Petrol, 5, 55.00m),
            CreateTestVehicle("Tesla Model 3", VehicleCategory.Luxus.Code, Locations.BerlinHauptbahnhof, FuelType.Electric, 5, 95.00m),
            CreateTestVehicle("Ford Focus", VehicleCategory.Kleinwagen.Code, Locations.MunichAirport, FuelType.Diesel, 5, 45.00m),
            CreateTestVehicle("BMW X5", VehicleCategory.SUV.Code, Locations.MunichAirport, FuelType.Diesel, 7, 120.00m)
        };

        await context.Vehicles.AddRangeAsync(vehicles);
        await context.SaveChangesAsync();
    }

    private Vehicle CreateTestVehicle(
        string name,
        string categoryCode,
        string locationCode,
        FuelType fuelType,
        int seats,
        decimal dailyRateGross)
    {
        var category = VehicleCategory.From(categoryCode);
        var location = LocationCode.From(locationCode);
        var dailyRateNet = dailyRateGross / 1.19m; // Convert gross to net (19% VAT)

        return VehicleBuilder.Default()
            .WithName(name)
            .WithCategory(category)
            .AtLocation(location)
            .WithDailyRate(dailyRateNet)
            .WithSeats(seats)
            .WithFuelType(fuelType)
            .WithTransmission(TransmissionType.Manual)
            .WithLicensePlate($"B AB {new Random().Next(1000, 9999)}")
            .Build();
    }
}
