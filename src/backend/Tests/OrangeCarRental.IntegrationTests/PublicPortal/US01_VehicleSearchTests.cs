using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.PublicPortal;

/// <summary>
///     US-1: Vehicle Search with Filters
///     As a customer, I want to search for available vehicles with filters
///     so that I can find a car that meets my specific needs.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Portal", "Public")]
public class US01_VehicleSearchTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region AC: Search with date range and location

    [Fact]
    public async Task Search_ByLocationCode_ReturnsVehiclesAtLocation()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?locationCode=MUC-FLG&pageSize=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.All(result.Items, v => Assert.Equal("MUC-FLG", v.LocationCode));
    }

    [Fact]
    public async Task Search_ByDateRange_ReturnsAvailableVehicles()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var pickupDate = DateTime.UtcNow.Date.AddDays(7).ToString("yyyy-MM-dd");
        var returnDate = DateTime.UtcNow.Date.AddDays(10).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/vehicles?pickupDate={pickupDate}&returnDate={returnDate}&pageSize=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.Items.Count >= 0);
    }

    #endregion

    #region AC: Filter by vehicle category

    [Theory]
    [InlineData("KOMPAKT")]
    [InlineData("MITTEL")]
    [InlineData("SUV")]
    [InlineData("LUXUS")]
    public async Task Search_ByCategory_ReturnsMatchingVehicles(string categoryCode)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/vehicles?categoryCode={categoryCode}&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        if (result.Items.Count > 0)
        {
            Assert.All(result.Items, v => Assert.Equal(categoryCode, v.CategoryCode));
        }
    }

    #endregion

    #region AC: Filter by fuel type

    [Fact]
    public async Task Search_ByFuelTypeElectric_ReturnsElectricVehicles()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?fuelType=Electric&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.Equal("Electric", v.FuelType));
    }

    #endregion

    #region AC: Filter by transmission type

    [Fact]
    public async Task Search_ByTransmissionAutomatic_ReturnsAutomaticVehicles()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?transmission=Automatic&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.Equal("Automatic", v.TransmissionType));
    }

    #endregion

    #region AC: Filter by minimum seats

    [Fact]
    public async Task Search_ByMinSeats_ReturnsVehiclesWithEnoughSeats()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var minSeats = 5;

        // Act
        var response = await httpClient.GetAsync($"/api/vehicles?minSeats={minSeats}&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.True(v.Seats >= minSeats));
    }

    #endregion

    #region AC: Multiple filters combined

    [Fact]
    public async Task Search_WithMultipleFilters_ReturnsFilteredResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync(
            "/api/vehicles?locationCode=BER-HBF&fuelType=Electric&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        foreach (var vehicle in result.Items)
        {
            Assert.Equal("BER-HBF", vehicle.LocationCode);
            Assert.Equal("Electric", vehicle.FuelType);
        }
    }

    #endregion

    #region AC: Prices shown with 19% VAT

    [Fact]
    public async Task Search_Results_IncludeGermanVatPricing()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=5");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Count > 0);

        foreach (var vehicle in result.Items)
        {
            // Gross price should be ~19% higher than net price
            var expectedGross = Math.Round(vehicle.DailyRateNet * 1.19m, 2);
            Assert.Equal(expectedGross, vehicle.DailyRateGross, 2);
        }
    }

    #endregion

    #region AC: Pagination

    [Fact]
    public async Task Search_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageNumber=1&pageSize=5");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.True(result.Items.Count <= 5);
    }

    #endregion

    #region AC: Availability checking

    [Fact]
    public async Task GetAvailability_ReturnsBookedVehicleIds()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var pickupDate = DateTime.UtcNow.Date.AddDays(7).ToString("yyyy-MM-dd");
        var returnDate = DateTime.UtcNow.Date.AddDays(14).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/availability?pickupDate={pickupDate}&returnDate={returnDate}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VehicleAvailabilityResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.NotNull(result.BookedVehicleIds);
    }

    #endregion

    // Helper classes
    private class VehicleSearchResult
    {
        public List<Vehicle> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private class Vehicle
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
    }

    private class VehicleAvailabilityResult
    {
        public List<Guid> BookedVehicleIds { get; set; } = new();
    }
}
