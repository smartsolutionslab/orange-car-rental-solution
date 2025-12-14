using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Integration tests for reservation availability checking
///     Tests the vehicle availability endpoint used by the Fleet service
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class ReservationAvailabilityTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetVehicleAvailability_ValidDateRange_ReturnsBookedVehicles()
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
        // Result could be empty if no reservations exist for that period
    }

    [Fact]
    public async Task GetVehicleAvailability_FarFutureDate_ReturnsEmptyOrFewBookings()
    {
        // Arrange - Use a date far in the future where few/no bookings should exist
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var pickupDate = DateTime.UtcNow.Date.AddYears(1).ToString("yyyy-MM-dd");
        var returnDate = DateTime.UtcNow.Date.AddYears(1).AddDays(7).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/availability?pickupDate={pickupDate}&returnDate={returnDate}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VehicleAvailabilityResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.NotNull(result.BookedVehicleIds);
    }

    [Fact]
    public async Task GetVehicleAvailability_SingleDayRental_Works()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var singleDate = DateTime.UtcNow.Date.AddDays(30).ToString("yyyy-MM-dd");
        var nextDay = DateTime.UtcNow.Date.AddDays(31).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/availability?pickupDate={singleDate}&returnDate={nextDay}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ReservationsApi_DirectAccess_Availability()
    {
        // Arrange - Access Reservations API directly (not through gateway)
        var httpClient = fixture.CreateHttpClient("reservations-api");
        var pickupDate = DateTime.UtcNow.Date.AddDays(5).ToString("yyyy-MM-dd");
        var returnDate = DateTime.UtcNow.Date.AddDays(10).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/availability?pickupDate={pickupDate}&returnDate={returnDate}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    // Helper class for deserialization
    private class VehicleAvailabilityResult
    {
        public List<Guid> BookedVehicleIds { get; set; } = new();
    }
}
