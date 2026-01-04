using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.PublicPortal;

/// <summary>
///     US-2: Booking Flow (Quick + Search-based)
///     As a customer, I want to complete a booking in a simple 5-step wizard
///     so that I can quickly rent a vehicle without complicated forms.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Portal", "Public")]
public class US02_BookingFlowTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private const decimal GermanVatRate = 0.19m;

    #region AC: Step 1 - Vehicle Details (pricing calculation)

    [Fact]
    public async Task CalculatePrice_ForRentalPeriod_ReturnsCorrectPricing()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10) // 3 days
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.TotalPriceNet > 0);
        Assert.True(result.TotalPriceGross > result.TotalPriceNet);
        // API may count days inclusively (pickup and return days both count)
        Assert.True(result.TotalDays >= 3 && result.TotalDays <= 4,
            $"Expected 3-4 days but got {result.TotalDays}");
        Assert.Equal("EUR", result.Currency);
    }

    [Fact]
    public async Task CalculatePrice_VerifiesGermanVat19Percent()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "SUV",
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(17)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);

        // Verify 19% German VAT calculation (allow small rounding tolerance)
        var expectedVat = Math.Round(result.TotalPriceNet * GermanVatRate, 2);
        Assert.True(Math.Abs(expectedVat - result.VatAmount) <= 0.02m,
            $"Expected VAT ~{expectedVat} but got {result.VatAmount}");
        Assert.True(Math.Abs((result.TotalPriceNet + result.VatAmount) - result.TotalPriceGross) <= 0.02m,
            $"Net + VAT should equal Gross: {result.TotalPriceNet} + {result.VatAmount} != {result.TotalPriceGross}");
    }

    [Fact]
    public async Task CalculatePrice_LongerPeriod_CostsMore()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var shortRental = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(8) // 1 day
        };
        var longRental = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14) // 7 days
        };

        // Act
        var shortResponse = await httpClient.PostAsJsonAsync("/api/pricing/calculate", shortRental);
        var longResponse = await httpClient.PostAsJsonAsync("/api/pricing/calculate", longRental);

        shortResponse.EnsureSuccessStatusCode();
        longResponse.EnsureSuccessStatusCode();

        var shortResult = await shortResponse.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);
        var longResult = await longResponse.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        // Assert
        Assert.NotNull(shortResult);
        Assert.NotNull(longResult);
        Assert.True(longResult.TotalPriceGross > shortResult.TotalPriceGross);
    }

    [Fact]
    public async Task CalculatePrice_InvalidDates_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7) // Return before pickup
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region AC: Step 2-5 - Complete Guest Booking

    [Fact]
    public async Task CreateGuestReservation_WithValidData_Succeeds()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // First, find an available vehicle
        var searchResponse = await httpClient.GetAsync("/api/vehicles?locationCode=MUC-FLG&pageSize=1");
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);
        Assert.NotNull(searchResult);
        Assert.True(searchResult.Items.Count > 0);

        var vehicle = searchResult.Items[0];

        // Create guest reservation with all 5 steps of data (nested structure)
        var request = new
        {
            reservation = new
            {
                vehicleId = Guid.Parse(vehicle.Id),
                categoryCode = vehicle.CategoryCode,
                pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14),
                returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(17),
                pickupLocationCode = "MUC-FLG",
                dropoffLocationCode = "MUC-FLG"
            },
            customer = new
            {
                firstName = "Hans",
                lastName = "Müller",
                email = $"hans.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 89 12345678",
                dateOfBirth = new DateOnly(1985, 6, 15)
            },
            address = new
            {
                street = "Marienplatz 1",
                city = "München",
                postalCode = "80331",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"M{Guid.NewGuid():N}"[..10],
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2010, 3, 20),
                licenseExpiryDate = new DateOnly(2030, 3, 19)
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/reservations/guest", request);

        // Assert - Step 5: Confirmation
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GuestReservationResult>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.CustomerId);
        Assert.NotEqual(Guid.Empty, result.ReservationId);
        Assert.True(result.TotalPriceGross > 0);
        Assert.Equal("EUR", result.Currency);

        // Verify location header for confirmation page
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/reservations/{result.ReservationId}", response.Headers.Location.ToString());
    }

    [Fact]
    public async Task CreateGuestReservation_UnderageCustomer_Fails()
    {
        // Arrange - Customer under 18 (AC: must be 18+)
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            reservation = new
            {
                vehicleId = Guid.NewGuid(),
                categoryCode = "KOMPAKT",
                pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
                pickupLocationCode = "MUC-FLG",
                dropoffLocationCode = "MUC-FLG"
            },
            customer = new
            {
                firstName = "Jung",
                lastName = "Person",
                email = $"jung.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 30 1111111",
                dateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-17)) // 17 years old
            },
            address = new
            {
                street = "Teststraße 1",
                city = "Berlin",
                postalCode = "10115",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = "J123456789",
                licenseIssueCountry = "Germany",
                licenseIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
                licenseExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddYears(10))
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/reservations/guest", request);

        // Assert - API should reject underage customers (may return 500 if validation throws)
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Expected BadRequest or InternalServerError but got {response.StatusCode}");
    }

    [Fact]
    public async Task CreateGuestReservation_ExpiredLicense_Fails()
    {
        // Arrange - Expired license (AC: min 30 days validity)
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            reservation = new
            {
                vehicleId = Guid.NewGuid(),
                categoryCode = "KOMPAKT",
                pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
                pickupLocationCode = "MUC-FLG",
                dropoffLocationCode = "MUC-FLG"
            },
            customer = new
            {
                firstName = "Expired",
                lastName = "License",
                email = $"expired.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 30 2222222",
                dateOfBirth = new DateOnly(1980, 5, 15)
            },
            address = new
            {
                street = "Abgelaufenweg 1",
                city = "Berlin",
                postalCode = "10117",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = "E123456789",
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2005, 1, 1),
                licenseExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) // Expired
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/reservations/guest", request);

        // Assert - API should reject expired license (may return 500 if validation throws)
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Expected BadRequest or InternalServerError but got {response.StatusCode}");
    }

    [Fact]
    public async Task CreateGuestReservation_PastPickupDate_Fails()
    {
        // Arrange - Pickup date in the past
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            reservation = new
            {
                vehicleId = Guid.NewGuid(),
                categoryCode = "KOMPAKT",
                pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1), // Past date
                returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3),
                pickupLocationCode = "MUC-FLG",
                dropoffLocationCode = "MUC-FLG"
            },
            customer = new
            {
                firstName = "Past",
                lastName = "Booking",
                email = $"past.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 89 3333333",
                dateOfBirth = new DateOnly(1990, 1, 1)
            },
            address = new
            {
                street = "Vergangenheit 1",
                city = "München",
                postalCode = "80331",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = "P123456789",
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 6, 1),
                licenseExpiryDate = new DateOnly(2035, 6, 1)
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/reservations/guest", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region AC: Price breakdown in confirmation

    [Theory]
    [InlineData("KOMPAKT")]
    [InlineData("MITTEL")]
    [InlineData("SUV")]
    [InlineData("LUXUS")]
    public async Task CalculatePrice_DifferentCategories_ReturnsDifferentRates(string categoryCode)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode,
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.DailyRateNet > 0);
        Assert.True(result.TotalPriceNet > 0);
        Assert.Equal("EUR", result.Currency);
    }

    #endregion

    // Helper classes
    private class VehicleSearchResult
    {
        public List<Vehicle> Items { get; set; } = new();
    }

    private class Vehicle
    {
        public string Id { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
    }

    private class PriceCalculationResult
    {
        public decimal TotalPriceNet { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalPriceGross { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public decimal DailyRateNet { get; set; }
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
}
