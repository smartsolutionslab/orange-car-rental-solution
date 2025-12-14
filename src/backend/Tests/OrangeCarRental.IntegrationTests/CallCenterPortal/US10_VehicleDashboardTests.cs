using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.CallCenterPortal;

/// <summary>
///     US-10: Dashboard with Vehicle Search
///     As a call center agent, I want to search for vehicles and view fleet status
///     so that I can help customers find available vehicles and manage fleet.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class US10_VehicleDashboardTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region AC: Dashboard statistics

    [Fact]
    public async Task GetAllVehicles_ReturnsVehicleList()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=100");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.True(result.TotalCount > 0, "Fleet should have vehicles");
    }

    [Fact]
    public async Task GetVehicles_CanCalculateStatusCounts()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - Get all vehicles to calculate stats
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=100");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert - Can calculate dashboard statistics
        Assert.NotNull(result);

        var availableCount = result.Items.Count(v => v.Status == "Available");
        var rentedCount = result.Items.Count(v => v.Status == "Rented");
        var maintenanceCount = result.Items.Count(v => v.Status == "Maintenance" || v.Status == "UnderMaintenance");

        Assert.True(availableCount + rentedCount + maintenanceCount <= result.TotalCount);
    }

    #endregion

    #region AC: Status filter

    [Theory]
    [InlineData("Available")]
    [InlineData("Rented")]
    public async Task FilterByStatus_ReturnsMatchingVehicles(string status)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/vehicles?status={status}&pageSize=50");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.Equal(status, v.Status));
    }

    #endregion

    #region AC: Location filter

    [Theory]
    [InlineData("BER-HBF")]
    [InlineData("MUC-FLG")]
    [InlineData("FRA-FLG")]
    public async Task FilterByLocation_ReturnsVehiclesAtLocation(string locationCode)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/vehicles?locationCode={locationCode}&pageSize=50");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.Equal(locationCode, v.LocationCode));
    }

    #endregion

    #region AC: Category filter

    [Theory]
    [InlineData("KOMPAKT")]
    [InlineData("SUV")]
    [InlineData("MITTEL")]
    public async Task FilterByCategory_ReturnsMatchingVehicles(string categoryCode)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/vehicles?categoryCode={categoryCode}&pageSize=50");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.Equal(categoryCode, v.CategoryCode));
    }

    #endregion

    #region AC: Vehicle details display

    [Fact]
    public async Task GetVehicles_ReturnsRequiredFields()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Count > 0);

        var vehicle = result.Items[0];

        // Verify required fields per AC
        Assert.NotEmpty(vehicle.Id);
        Assert.NotEmpty(vehicle.Name);
        Assert.NotEmpty(vehicle.CategoryCode);
        Assert.NotEmpty(vehicle.LocationCode);
        Assert.NotEmpty(vehicle.FuelType);
        Assert.NotEmpty(vehicle.TransmissionType);
        Assert.True(vehicle.Seats > 0);
        Assert.True(vehicle.DailyRateNet > 0);
        Assert.True(vehicle.DailyRateGross > 0);
        Assert.NotEmpty(vehicle.Status);
    }

    [Fact]
    public async Task GetVehicles_IncludesLicensePlate()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        // At least some vehicles should have license plates
        var vehiclesWithPlates = result.Items.Where(v => !string.IsNullOrEmpty(v.LicensePlate)).ToList();
        Assert.NotEmpty(vehiclesWithPlates);
    }

    #endregion

    #region AC: Multiple filters combined

    [Fact]
    public async Task CombineFilters_ReturnsMatchingResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - Filter by location, status, and category
        var response = await httpClient.GetAsync(
            "/api/vehicles?locationCode=MUC-FLG&status=Available&pageSize=50");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v =>
        {
            Assert.Equal("MUC-FLG", v.LocationCode);
            Assert.Equal("Available", v.Status);
        });
    }

    #endregion

    #region AC: Result count

    [Fact]
    public async Task Search_ReturnsFilteredCountVsTotal()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Get total count
        var allResponse = await httpClient.GetAsync("/api/vehicles?pageSize=100");
        allResponse.EnsureSuccessStatusCode();
        var allResult = await allResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Get filtered count
        var filteredResponse = await httpClient.GetAsync("/api/vehicles?status=Available&pageSize=100");
        filteredResponse.EnsureSuccessStatusCode();
        var filteredResult = await filteredResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(allResult);
        Assert.NotNull(filteredResult);
        Assert.True(filteredResult.TotalCount <= allResult.TotalCount);
    }

    #endregion

    #region AC: Pricing with German VAT

    [Fact]
    public async Task GetVehicles_PricesInclude19PercentVat()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        foreach (var vehicle in result.Items)
        {
            var expectedGross = Math.Round(vehicle.DailyRateNet * 1.19m, 2);
            Assert.Equal(expectedGross, vehicle.DailyRateGross, 2);
        }
    }

    #endregion

    // Helper classes
    private class VehicleSearchResult
    {
        public List<VehicleDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private class VehicleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public string TransmissionType { get; set; } = string.Empty;
        public int Seats { get; set; }
        public decimal DailyRateNet { get; set; }
        public decimal DailyRateGross { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? LicensePlate { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
    }
}
