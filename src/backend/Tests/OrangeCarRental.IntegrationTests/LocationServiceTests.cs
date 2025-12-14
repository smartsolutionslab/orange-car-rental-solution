using System.Net;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Integration tests for Location API endpoints through the API Gateway
///     Tests the location service functionality for retrieving rental locations
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class LocationServiceTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetAllLocations_ReturnsActiveLocations()
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
        Assert.True(result.Locations.Count > 0, "Expected at least one location");

        // Verify location structure
        var firstLocation = result.Locations[0];
        Assert.NotEmpty(firstLocation.Code);
        Assert.NotEmpty(firstLocation.Name);
        Assert.NotEmpty(firstLocation.City);
    }

    [Fact]
    public async Task GetAllLocations_ReturnsGermanLocations()
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
        Assert.NotNull(result.Locations);

        var locationCodes = result.Locations.Select(l => l.Code).ToList();
        foreach (var expectedCode in expectedLocations)
        {
            Assert.Contains(expectedCode, locationCodes);
        }
    }

    [Fact]
    public async Task GetLocationByCode_ReturnsLocationDetails()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var locationCode = "MUC-FLG";

        // Act
        var response = await httpClient.GetAsync($"/api/locations/{locationCode}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var location = JsonSerializer.Deserialize<LocationDto>(content, JsonOptions);

        Assert.NotNull(location);
        Assert.Equal(locationCode, location.Code);
        Assert.Contains("München", location.City);
    }

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

    [Fact]
    public async Task GetAllLocations_EachLocationHasValidAddress()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/locations");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LocationListResult>(content, JsonOptions);

        // Assert
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

    [Fact]
    public async Task LocationApi_DirectAccess_ReturnsLocations()
    {
        // Arrange - Access Fleet API directly (not through gateway)
        var httpClient = fixture.CreateHttpClient("fleet-api");

        // Act
        var response = await httpClient.GetAsync("/api/locations");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("BER-HBF", content);
    }

    // Helper classes for deserialization
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
}
