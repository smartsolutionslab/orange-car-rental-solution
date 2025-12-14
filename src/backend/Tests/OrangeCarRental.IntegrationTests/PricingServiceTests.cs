using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Integration tests for Pricing API endpoints
///     Tests the pricing calculation service with German VAT (19%)
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class PricingServiceTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private const decimal GermanVatRate = 0.19m;

    [Fact]
    public async Task CalculatePrice_ValidRequest_ReturnsCorrectPricing()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(10)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.TotalPriceNet > 0, "Net price should be greater than 0");
        Assert.True(result.TotalPriceVat > 0, "VAT should be greater than 0");
        Assert.True(result.TotalPriceGross > 0, "Gross price should be greater than 0");
        Assert.Equal("EUR", result.Currency);
    }

    [Fact]
    public async Task CalculatePrice_VerifiesGermanVatCalculation()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateTime.UtcNow.Date.AddDays(14),
            returnDate = DateTime.UtcNow.Date.AddDays(17)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);

        // Verify VAT calculation (19% German VAT)
        var expectedVat = Math.Round(result.TotalPriceNet * GermanVatRate, 2);
        Assert.Equal(expectedVat, result.TotalPriceVat, 2);

        // Verify gross = net + vat
        Assert.Equal(result.TotalPriceNet + result.TotalPriceVat, result.TotalPriceGross, 2);
    }

    [Theory]
    [InlineData("KOMPAKT")]
    [InlineData("MITTEL")]
    [InlineData("SUV")]
    [InlineData("ECAR")]
    [InlineData("LUXUS")]
    public async Task CalculatePrice_DifferentCategories_ReturnsDifferentPrices(string categoryCode)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode,
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(10)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.TotalPriceNet > 0);
        Assert.Equal("EUR", result.Currency);
    }

    [Fact]
    public async Task CalculatePrice_LongerRentalPeriod_HigherPrice()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var shortRental = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(8) // 1 day
        };
        var longRental = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(14) // 7 days
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
        Assert.True(longResult.TotalPriceGross > shortResult.TotalPriceGross,
            "Longer rental should cost more");
    }

    [Fact]
    public async Task CalculatePrice_WithLocationCode_AppliesLocationPricing()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "SUV",
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(10),
            locationCode = "MUC-FLG"
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.TotalPriceNet > 0);
    }

    [Fact]
    public async Task CalculatePrice_InvalidCategory_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "INVALID_CATEGORY",
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(10)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CalculatePrice_ReturnDateBeforePickup_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateTime.UtcNow.Date.AddDays(10),
            returnDate = DateTime.UtcNow.Date.AddDays(7) // Before pickup
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PricingApi_DirectAccess_CalculatesPrice()
    {
        // Arrange - Access Pricing API directly (not through gateway)
        var httpClient = fixture.CreateHttpClient("pricing-api");
        var request = new
        {
            categoryCode = "ECAR",
            pickupDate = DateTime.UtcNow.Date.AddDays(5),
            returnDate = DateTime.UtcNow.Date.AddDays(8)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PriceCalculationResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.TotalPriceNet > 0);
        Assert.Equal("EUR", result.Currency);
    }

    // Helper class for deserialization
    private class PriceCalculationResult
    {
        public decimal TotalPriceNet { get; set; }
        public decimal TotalPriceVat { get; set; }
        public decimal TotalPriceGross { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int RentalDays { get; set; }
        public decimal DailyRateNet { get; set; }
    }
}
