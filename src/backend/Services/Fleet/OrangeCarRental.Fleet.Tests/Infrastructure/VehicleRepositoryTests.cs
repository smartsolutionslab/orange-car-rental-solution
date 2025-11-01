using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Infrastructure;

public class VehicleRepositoryTests : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;
    private FleetDbContext _context = null!;
    private SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence.ReservationsDbContext _reservationsContext = null!;
    private VehicleRepository _repository = null!;

    public VehicleRepositoryTests()
    {
        // Configure SQL Server container
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the SQL Server container
        await _msSqlContainer.StartAsync();

        // Create DbContext with SQL Server connection
        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString())
            .Options;

        _context = new FleetDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        // Create Reservations DbContext with same SQL Server connection
        var reservationsOptions = new DbContextOptionsBuilder<SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence.ReservationsDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString())
            .Options;

        _reservationsContext = new SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence.ReservationsDbContext(reservationsOptions);
        await _reservationsContext.Database.EnsureCreatedAsync();

        _repository = new VehicleRepository(_context, _reservationsContext);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _reservationsContext.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAllAvailableVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(5); // All 5 test vehicles
        result.TotalCount.Should().Be(5);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task SearchAsync_WithLocationFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            LocationCode = "BER-HBF",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(3);
        result.Items.Should().AllSatisfy(v =>
            v.CurrentLocation.Code.Value.Should().Be("BER-HBF"));
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task SearchAsync_WithCategoryFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            CategoryCode = "KLEIN",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(v =>
            v.Category.Code.Should().Be("KLEIN"));
    }

    [Fact]
    public async Task SearchAsync_WithFuelTypeFilter_ReturnsOnlyMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            FuelType = FuelType.Electric,
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().FuelType.Should().Be(FuelType.Electric);
    }

    [Fact]
    public async Task SearchAsync_WithMinSeatsFilter_ReturnsVehiclesWithEnoughSeats()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            MinSeats = 5,
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        // Test data has: VW Golf (5), Tesla (5), Ford Focus (5), BMW X5 (7) = 4 vehicles with >= 5 seats
        result.Items.Should().HaveCount(4);
        result.Items.Should().AllSatisfy(v =>
            v.Seats.Value.Should().BeGreaterThanOrEqualTo(5));
    }

    [Fact]
    public async Task SearchAsync_WithMaxPriceFilter_ReturnsVehiclesWithinBudget()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            MaxDailyRateGross = 50.00m,
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(v =>
            v.DailyRate.GrossAmount.Should().BeLessThanOrEqualTo(50.00m));
    }

    [Fact]
    public async Task SearchAsync_WithMultipleFilters_ReturnsMatchingVehicles()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            LocationCode = "BER-HBF",
            FuelType = FuelType.Petrol,
            MinSeats = 4,
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(v =>
        {
            v.CurrentLocation.Code.Value.Should().Be("BER-HBF");
            v.FuelType.Should().Be(FuelType.Petrol);
            v.Seats.Value.Should().BeGreaterThanOrEqualTo(4);
        });
    }

    [Fact]
    public async Task SearchAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await SeedTestDataAsync();
        var parameters = new VehicleSearchParameters
        {
            PageNumber = 2,
            PageSize = 2
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task SearchAsync_WithStatusFilter_ReturnsOnlyAvailableVehicles()
    {
        // Arrange
        await SeedTestDataAsync();

        // Mark one vehicle as rented
        var vehicle = await _context.Vehicles.FirstAsync();
        vehicle.ChangeStatus(VehicleStatus.Rented);
        await _context.SaveChangesAsync();

        var parameters = new VehicleSearchParameters
        {
            Status = VehicleStatus.Available,
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _repository.SearchAsync(parameters, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(4);
        result.Items.Should().AllSatisfy(v =>
            v.Status.Should().Be(VehicleStatus.Available));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsVehicle()
    {
        // Arrange
        var vehicle = CreateTestVehicle("VW Polo", "KLEIN", "BER-HBF", FuelType.Petrol, 4, 35.00m);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(vehicle.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(vehicle.Id);
        result.Name.Value.Should().Be("VW Polo");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = VehicleIdentifier.New();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
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

        await _context.Vehicles.AddRangeAsync(vehicles);
        await _context.SaveChangesAsync();
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
