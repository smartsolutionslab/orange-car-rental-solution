using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     End-to-end integration tests for complete user scenarios
///     Tests the entire flow from searching vehicles to creating reservations
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class EndToEndScenarioTests(DistributedApplicationFixture fixture)
{
    [Fact]
    public async Task VehicleSearchFlow_SearchByLocationReturnsVehicles()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - Search for available vehicles at Berlin Hauptbahnhof
        var searchResponse = await httpClient.GetAsync("/api/vehicles?locationCode=BER-HBF");
        searchResponse.EnsureSuccessStatusCode();

        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<VehicleSearchResult>(searchContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.NotNull(searchResult);
        Assert.NotNull(searchResult.Vehicles);
        Assert.True(searchResult.Vehicles.Count > 0, "Expected at least one vehicle at BER-HBF location");

        // Verify all returned vehicles are at the requested location
        foreach (var vehicle in searchResult.Vehicles)
        {
            Assert.Equal("BER-HBF", vehicle.LocationCode);
            Assert.NotEmpty(vehicle.Id);
            Assert.NotEmpty(vehicle.Name);
            Assert.NotEmpty(vehicle.CategoryCode);
        }
    }

    [Fact]
    public async Task SearchVehicles_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - Search with multiple filters
        // Using KOMPAKT (German for compact) which is the actual category code
        var response = await httpClient.GetAsync(
            "/api/vehicles?locationCode=MUC-FLG&categoryCode=KOMPAKT&fuelType=Petrol");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<VehicleSearchResult>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.NotNull(result.Vehicles);

        // All returned vehicles should match the filters
        foreach (var vehicle in result.Vehicles)
        {
            Assert.Equal("MUC-FLG", vehicle.LocationCode);
            Assert.Equal("KOMPAKT", vehicle.CategoryCode);
            Assert.Equal("Petrol", vehicle.FuelType);
        }
    }

    [Fact]
    public async Task VehicleSearch_IncludesGermanVAT_InPricing()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<VehicleSearchResult>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.NotNull(result.Vehicles);

        if (result.Vehicles.Count > 0)
        {
            var vehicle = result.Vehicles[0];

            // Verify pricing structure includes VAT
            Assert.True(vehicle.DailyRateNet > 0);
            Assert.True(vehicle.DailyRateVat > 0);
            Assert.True(vehicle.DailyRateGross > 0);

            // Verify gross = net + vat
            Assert.Equal(vehicle.DailyRateGross, vehicle.DailyRateNet + vehicle.DailyRateVat, 2);

            // Verify 19% German VAT
            var expectedVat = Math.Round(vehicle.DailyRateNet * 0.19m, 2);
            Assert.Equal(expectedVat, vehicle.DailyRateVat, 2);
        }
    }

    // Helper classes for deserialization
    private class VehicleSearchResult
    {
        public List<Vehicle> Vehicles { get; set; } = new();
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
        public decimal DailyRateNet { get; set; }
        public decimal DailyRateVat { get; set; }
        public decimal DailyRateGross { get; set; }
    }
}
