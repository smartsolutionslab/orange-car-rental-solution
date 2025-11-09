using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     End-to-end integration tests for complete user scenarios
///     Tests the entire flow from searching vehicles to creating reservations
/// </summary>
public class EndToEndScenarioTests(DistributedApplicationFixture fixture) : IClassFixture<DistributedApplicationFixture>
{
    [Fact]
    public async Task CompleteRentalFlow_SearchVehicleAndCreateReservation()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Step 1: Search for available vehicles at Berlin Hauptbahnhof
        var searchResponse = await httpClient.GetAsync("/api/vehicles?locationCode=BER-HBF");
        searchResponse.EnsureSuccessStatusCode();

        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<VehicleSearchResult>(searchContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(searchResult);
        Assert.NotNull(searchResult.Vehicles);

        // If we have vehicles, proceed with reservation
        if (searchResult.Vehicles.Count > 0)
        {
            var selectedVehicle = searchResult.Vehicles[0];

            // Step 2: Create a reservation
            var reservationCommand = new
            {
                vehicleId = selectedVehicle.Id,
                customerId = Guid.NewGuid(),
                categoryCode = selectedVehicle.CategoryCode,
                pickupDate = DateTime.UtcNow.AddDays(1).ToString("O"),
                returnDate = DateTime.UtcNow.AddDays(3).ToString("O"),
                pickupLocationCode = "BER-HBF",
                dropoffLocationCode = "BER-HBF"
                // Note: totalPriceNet is optional - will be calculated automatically by Pricing Service
            };

            var createResponse = await httpClient.PostAsJsonAsync("/api/reservations", reservationCommand);

            // If the response is not Created, log the error details for debugging
            if (createResponse.StatusCode != HttpStatusCode.Created)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                throw new Exception(
                    $"Failed to create reservation. Status: {createResponse.StatusCode}, Error: {errorContent}");
            }

            // Assert reservation was created
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var location = createResponse.Headers.Location;
            Assert.NotNull(location);

            // Step 3: Retrieve the created reservation
            var getResponse = await httpClient.GetAsync(location);
            getResponse.EnsureSuccessStatusCode();

            var reservationContent = await getResponse.Content.ReadAsStringAsync();
            Assert.Contains(selectedVehicle.Id, reservationContent);
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
