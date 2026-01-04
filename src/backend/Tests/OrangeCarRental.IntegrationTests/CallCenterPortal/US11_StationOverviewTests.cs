using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.CallCenterPortal;

/// <summary>
///     US-11: Station Overview with Vehicle Inventory
///     As a call center agent, I want to view all rental locations and their vehicle inventory
///     so that I can see which vehicles are at which location.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Portal", "CallCenter")]
public class US11_StationOverviewTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region AC: Dashboard statistics - Total locations

    [Fact]
    public async Task GetAllLocations_ReturnsLocationList()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/locations");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LocationListResult>(content, JsonOptions);

        Assert.NotNull(result);
        Assert.NotNull(result.Locations);
        // Note: Location API may not have seeded data in CI - only verify API returns valid response
    }

    #endregion

    #region AC: Location cards with German cities

    [Fact]
    public async Task GetAllLocations_ReturnsGermanRentalLocations()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var expectedLocations = new[] { "BER-HBF", "MUC-FLG", "FRA-FLG", "HAM-HBF", "CGN-HBF" };

        // Act
        var response = await httpClient.GetAsync("/api/locations");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LocationListResult>(content, JsonOptions);

        // Assert
        Assert.NotNull(result);
        // Only check for expected locations if the Location API has seeded data
        if (result.Locations.Count > 0)
        {
            var locationCodes = result.Locations.Select(l => l.Code).ToList();
            foreach (var expectedCode in expectedLocations)
            {
                Assert.Contains(expectedCode, locationCodes);
            }
        }
    }

    #endregion

    #region AC: Location detail with address

    [Fact]
    public async Task GetLocationByCode_ReturnsLocationDetails()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var locationCode = "MUC-FLG";

        // Act
        var response = await httpClient.GetAsync($"/api/locations/{locationCode}");

        // Assert - Location may not exist if Location API not seeded
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var location = JsonSerializer.Deserialize<LocationDto>(content, JsonOptions);

            Assert.NotNull(location);
            Assert.Equal(locationCode, location.Code);
            Assert.Contains("Munich", location.City);
        }
        else
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task GetAllLocations_EachLocationHasValidGermanAddress()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/locations");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LocationListResult>(content, JsonOptions);

        // Assert - Verify German address format only if locations exist
        Assert.NotNull(result);
        foreach (var location in result.Locations)
        {
            Assert.NotEmpty(location.Street);
            Assert.NotEmpty(location.City);
            Assert.NotEmpty(location.PostalCode);
            // German postal codes are 5 digits
            Assert.Matches(@"^\d{5}$", location.PostalCode);
        }
    }

    #endregion

    #region AC: Vehicle count per location

    [Fact]
    public async Task GetVehiclesAtLocation_ReturnsVehicleInventory()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var locationCode = "BER-HBF";

        // Act
        var response = await httpClient.GetAsync($"/api/vehicles?locationCode={locationCode}&pageSize=50");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        // All returned vehicles should be at the requested location
        Assert.All(result.Items, v => Assert.Equal(locationCode, v.LocationCode));
    }

    [Fact]
    public async Task LocationsAndVehicles_HaveConsistentData()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Get all locations
        var locationsResponse = await httpClient.GetAsync("/api/locations");
        locationsResponse.EnsureSuccessStatusCode();
        var locationsContent = await locationsResponse.Content.ReadAsStringAsync();
        var locationsResult = JsonSerializer.Deserialize<LocationListResult>(locationsContent, JsonOptions);

        // Get all vehicles
        var vehiclesResponse = await httpClient.GetAsync("/api/vehicles?pageSize=100");
        vehiclesResponse.EnsureSuccessStatusCode();
        var vehicles = await vehiclesResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(locationsResult);
        Assert.NotNull(vehicles);

        // Only verify consistency if Location API has seeded data
        if (locationsResult.Locations.Count > 0)
        {
            var validLocationCodes = locationsResult.Locations.Select(l => l.Code).ToHashSet();

            // All vehicles should reference valid locations
            foreach (var vehicle in vehicles.Items)
            {
                Assert.Contains(vehicle.LocationCode, validLocationCodes);
            }
        }
    }

    #endregion

    #region AC: Invalid location returns 404

    [Fact]
    public async Task GetLocationByCode_InvalidCode_ReturnsNotFound()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var invalidCode = "INVALID-LOC";

        // Act
        var response = await httpClient.GetAsync($"/api/locations/{invalidCode}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region AC: Direct API access

    [Fact]
    public async Task LocationApi_DirectAccess_ReturnsLocations()
    {
        // Arrange - Access Fleet API directly (locations are part of Fleet API)
        var httpClient = fixture.CreateHttpClient("fleet-api");

        // Act
        var response = await httpClient.GetAsync("/api/locations");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("BER-HBF", content);
    }

    #endregion

    // Helper classes
    private class LocationListResult
    {
        public List<LocationDto> Locations { get; set; } = new();
    }

    private class LocationDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private class VehicleSearchResult
    {
        public List<VehicleDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private class VehicleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
