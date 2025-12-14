using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Integration tests for complete booking flow scenarios
///     Tests the entire journey from vehicle search to reservation creation
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class FullBookingFlowTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task FullGuestBookingFlow_SearchToReservation_Succeeds()
    {
        // This test covers the complete guest booking flow:
        // 1. Search for available vehicles
        // 2. Get pricing for selected vehicle
        // 3. Create guest reservation (customer + reservation in one call)

        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Step 1: Search for available vehicles at Munich Airport
        var searchResponse = await httpClient.GetAsync("/api/vehicles?locationCode=MUC-FLG&pageSize=5");
        searchResponse.EnsureSuccessStatusCode();

        var searchResult = await searchResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);
        Assert.NotNull(searchResult);
        Assert.True(searchResult.Items.Count > 0, "Expected at least one vehicle at MUC-FLG");

        var selectedVehicle = searchResult.Items[0];

        // Step 2: Get pricing for the rental period
        var pricingRequest = new
        {
            categoryCode = selectedVehicle.CategoryCode,
            pickupDate = DateTime.UtcNow.Date.AddDays(14),
            returnDate = DateTime.UtcNow.Date.AddDays(17),
            locationCode = "MUC-FLG"
        };

        var pricingResponse = await httpClient.PostAsJsonAsync("/api/pricing/calculate", pricingRequest);
        pricingResponse.EnsureSuccessStatusCode();

        var pricing = await pricingResponse.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);
        Assert.NotNull(pricing);
        Assert.True(pricing.TotalPriceGross > 0);

        // Step 3: Create guest reservation
        var guestReservationRequest = new
        {
            vehicleId = Guid.Parse(selectedVehicle.Id),
            categoryCode = selectedVehicle.CategoryCode,
            pickupDate = DateTime.UtcNow.Date.AddDays(14),
            returnDate = DateTime.UtcNow.Date.AddDays(17),
            pickupLocationCode = "MUC-FLG",
            dropoffLocationCode = "MUC-FLG",
            firstName = "Integration",
            lastName = "Test",
            email = $"integration.{Guid.NewGuid():N}@test.de",
            phoneNumber = "+49 89 12345678",
            dateOfBirth = new DateOnly(1990, 5, 15),
            street = "Teststraße 123",
            city = "München",
            postalCode = "80331",
            country = "Germany",
            licenseNumber = $"IT{Guid.NewGuid():N}".Substring(0, 10),
            licenseIssueCountry = "Germany",
            licenseIssueDate = new DateOnly(2015, 6, 1),
            licenseExpiryDate = new DateOnly(2035, 6, 1)
        };

        var reservationResponse = await httpClient.PostAsJsonAsync("/api/reservations/guest", guestReservationRequest);
        Assert.Equal(HttpStatusCode.Created, reservationResponse.StatusCode);

        var reservation = await reservationResponse.Content.ReadFromJsonAsync<GuestReservationResult>(JsonOptions);
        Assert.NotNull(reservation);
        Assert.NotEqual(Guid.Empty, reservation.CustomerId);
        Assert.NotEqual(Guid.Empty, reservation.ReservationId);
        Assert.True(reservation.TotalPriceGross > 0);
        Assert.Equal("EUR", reservation.Currency);

        // Verify location header points to the new reservation
        Assert.NotNull(reservationResponse.Headers.Location);
        Assert.Contains($"/api/reservations/{reservation.ReservationId}",
            reservationResponse.Headers.Location.ToString());
    }

    [Fact]
    public async Task SearchVehicles_FilterByCategory_ReturnsMatchingVehicles()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - Search for SUVs only
        var response = await httpClient.GetAsync("/api/vehicles?categoryCode=SUV&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);

        if (result.Items.Count > 0)
        {
            Assert.All(result.Items, v => Assert.Equal("SUV", v.CategoryCode));
        }
    }

    [Fact]
    public async Task SearchVehicles_FilterByMultipleCriteria_ReturnsFilteredResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - Search with multiple filters
        var response = await httpClient.GetAsync(
            "/api/vehicles?locationCode=BER-HBF&fuelType=Electric&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);

        foreach (var vehicle in result.Items)
        {
            Assert.Equal("BER-HBF", vehicle.LocationCode);
            Assert.Equal("Electric", vehicle.FuelType);
        }
    }

    [Fact]
    public async Task LocationsAndVehicles_ConsistentData()
    {
        // This test verifies that vehicles are associated with valid locations
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Get all locations
        var locationsResponse = await httpClient.GetAsync("/api/locations");
        locationsResponse.EnsureSuccessStatusCode();
        var locationsContent = await locationsResponse.Content.ReadAsStringAsync();
        var locations = JsonSerializer.Deserialize<LocationListResult>(locationsContent, JsonOptions);

        // Get all vehicles
        var vehiclesResponse = await httpClient.GetAsync("/api/vehicles?pageSize=100");
        vehiclesResponse.EnsureSuccessStatusCode();
        var vehicles = await vehiclesResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(locations);
        Assert.NotNull(vehicles);

        var validLocationCodes = locations.Locations.Select(l => l.Code).ToHashSet();

        foreach (var vehicle in vehicles.Items)
        {
            Assert.Contains(vehicle.LocationCode, validLocationCodes);
        }
    }

    [Fact]
    public async Task AllServices_AreHealthy_AndAccessible()
    {
        // This test verifies all services are running and healthy
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Check gateway health
        var gatewayHealth = await httpClient.GetAsync("/health");
        gatewayHealth.EnsureSuccessStatusCode();

        // Check Fleet API via gateway
        var fleetHealth = await httpClient.GetAsync("/api/vehicles/health");
        fleetHealth.EnsureSuccessStatusCode();

        // Check Reservations API via gateway
        var reservationsHealth = await httpClient.GetAsync("/api/reservations/health");
        reservationsHealth.EnsureSuccessStatusCode();

        // Check Pricing API directly (it doesn't have a gateway route for health)
        var pricingClient = fixture.CreateHttpClient("pricing-api");
        var pricingHealth = await pricingClient.GetAsync("/health");
        pricingHealth.EnsureSuccessStatusCode();

        // Check Customers API directly
        var customersClient = fixture.CreateHttpClient("customers-api");
        var customersHealth = await customersClient.GetAsync("/health");
        customersHealth.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GuestReservation_WithInvalidDates_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        var invalidRequest = new
        {
            vehicleId = Guid.NewGuid(),
            categoryCode = "KOMPAKT",
            pickupDate = DateTime.UtcNow.Date.AddDays(-1), // Past date
            returnDate = DateTime.UtcNow.Date.AddDays(3),
            pickupLocationCode = "MUC-FLG",
            dropoffLocationCode = "MUC-FLG",
            firstName = "Invalid",
            lastName = "Booking",
            email = $"invalid.{Guid.NewGuid():N}@test.de",
            phoneNumber = "+49 89 12345678",
            dateOfBirth = new DateOnly(1990, 5, 15),
            street = "Teststraße 123",
            city = "München",
            postalCode = "80331",
            country = "Germany",
            licenseNumber = "INV1234567",
            licenseIssueCountry = "Germany",
            licenseIssueDate = new DateOnly(2015, 6, 1),
            licenseExpiryDate = new DateOnly(2035, 6, 1)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/reservations/guest", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Helper classes for deserialization
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
        public decimal DailyRateNet { get; set; }
        public decimal DailyRateGross { get; set; }
    }

    private class PriceCalculationResult
    {
        public decimal TotalPriceNet { get; set; }
        public decimal TotalPriceVat { get; set; }
        public decimal TotalPriceGross { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    private class GuestReservationResult
    {
        public Guid CustomerId { get; set; }
        public Guid ReservationId { get; set; }
        public decimal TotalPriceNet { get; set; }
        public decimal TotalPriceVat { get; set; }
        public decimal TotalPriceGross { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    private class LocationListResult
    {
        public List<Location> Locations { get; set; } = new();
    }

    private class Location
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
