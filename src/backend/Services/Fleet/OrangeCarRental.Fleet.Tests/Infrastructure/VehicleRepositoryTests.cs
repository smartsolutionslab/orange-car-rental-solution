using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Infrastructure;

public class VehicleRepositoryTests : IAsyncLifetime
{
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private FleetDbContext context = null!;
    private VehicleRepository repository = null!;
    private ReservationsDbContext reservationsContext = null!;

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

        // Create Reservations DbContext with same SQL Server connection
        var reservationsOptions = new DbContextOptionsBuilder<ReservationsDbContext>()
            .UseSqlServer(msSqlContainer.GetConnectionString())
            .Options;

        reservationsContext = new ReservationsDbContext(reservationsOptions);
        await reservationsContext.Database.EnsureCreatedAsync();

        repository = new VehicleRepository(context, reservationsContext);
    }

    public async Task DisposeAsync()
    {
        await context.DisposeAsync();
        await reservationsContext.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAllAvailableVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(pageNumber: 1, pageSize: 20);

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
        var parameters =
            new VehicleSearchParameters(locationCode: LocationCode.Of("BER-HBF"), pageNumber: 1, pageSize: 20);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(3);
        result.Items.ShouldAllBe(v => v.CurrentLocation.Code.Value == "BER-HBF");
        result.TotalCount.ShouldBe(3);
    }

    [Fact]
    public async Task SearchAsync_WithCategoryFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters =
            new VehicleSearchParameters(category: VehicleCategory.FromCode("KLEIN"), pageNumber: 1, pageSize: 20);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(v => v.Category.Code == "KLEIN");
    }

    [Fact]
    public async Task SearchAsync_WithFuelTypeFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(fuelType: FuelType.Electric, pageNumber: 1, pageSize: 20);

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
        var parameters = new VehicleSearchParameters(minSeats: 5, pageNumber: 1, pageSize: 20);

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
        var parameters = new VehicleSearchParameters(maxDailyRateGross: 50.00m, pageNumber: 1, pageSize: 20);

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
        var parameters = new VehicleSearchParameters(locationCode: LocationCode.Of("BER-HBF"),
            fuelType: FuelType.Petrol, minSeats: 4, pageNumber: 1, pageSize: 20);

        // Act
        var result = await repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(2);
        foreach (var v in result.Items)
        {
            v.CurrentLocation.Code.Value.ShouldBe("BER-HBF");
            v.FuelType.ShouldBe(FuelType.Petrol);
            v.Seats.Value.ShouldBeGreaterThanOrEqualTo(4);
        }
    }

    [Fact]
    public async Task SearchAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters(pageNumber: 2, pageSize: 2);

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

        var parameters = new VehicleSearchParameters(status: VehicleStatus.Available, pageNumber: 1, pageSize: 20);

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
        var vehicle = CreateTestVehicle("VW Polo", "KLEIN", "BER-HBF", FuelType.Petrol, 4, 35.00m);
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

    private async Task SeedTestDataAsync()
    {
        var vehicles = new[]
        {
            CreateTestVehicle("VW Polo", "KLEIN", "BER-HBF", FuelType.Petrol, 4, 35.00m),
            CreateTestVehicle("VW Golf", "MITTEL", "BER-HBF", FuelType.Petrol, 5, 55.00m),
            CreateTestVehicle("Tesla Model 3", "LUXUS", "BER-HBF", FuelType.Electric, 5, 95.00m),
            CreateTestVehicle("Ford Focus", "KLEIN", "MUC-FLG", FuelType.Diesel, 5, 45.00m),
            CreateTestVehicle("BMW X5", "SUV", "MUC-FLG", FuelType.Diesel, 7, 120.00m)
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
        var category = VehicleCategory.FromCode(categoryCode);
        var location = Location.Of(locationCode, "Test City");
        var currency = Currency.Of("EUR");

        // Create money from gross amount
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
        vehicle.SetLicensePlate($"B-{Guid.NewGuid().ToString()[..6].ToUpper()}");
        return vehicle;
    }
}
