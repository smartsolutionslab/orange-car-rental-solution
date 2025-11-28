using Moq;
using SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;
using SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Application.Queries;

public class CalculatePriceQueryHandlerTests
{
    private readonly Mock<IPricingPolicyRepository> pricingPolicyRepositoryMock = new();
    private readonly CalculatePriceQueryHandler handler;

    public CalculatePriceQueryHandlerTests()
    {
        handler = new CalculatePriceQueryHandler(pricingPolicyRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldCalculatePrice()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

        var query = new CalculatePriceQuery(
            CategoryCode.From("KLEIN"),
            pickupDate,
            returnDate, null);

        var pricingPolicy = PricingPolicyBuilder.Default()
            .WithCategory("KLEIN")
            .WithDailyRate(50.00m)
            .Build();

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pricingPolicy);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CategoryCode.ShouldBe("KLEIN");
        result.TotalDays.ShouldBe(4); // 4 days
        result.DailyRateNet.ShouldBe(50.00m);
        result.TotalPriceNet.ShouldBe(200.00m); // 4 days * 50.00
        result.Currency.ShouldBe("EUR");
        result.PickupDate.ShouldBe(pickupDate);
        result.ReturnDate.ShouldBe(returnDate);

        pricingPolicyRepositoryMock.Verify(
            x => x.GetActivePolicyByCategoryAsync(query.CategoryCode, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithLocationCode_ShouldTryLocationSpecificPricingFirst()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        var locationCode = LocationCode.From("BER-HBF");

        var query = new CalculatePriceQuery(
            CategoryCode.From("SUV"),
            pickupDate,
            returnDate,
            locationCode);

        var locationPolicy = PricingPolicyBuilder.Default()
            .AsSuv()
            .WithDailyRate(80.00m)
            .ForLocation("BER-HBF")
            .Build();

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAndLocationAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(locationPolicy);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.TotalPriceNet.ShouldBe(240.00m); // 3 days * 80.00

        pricingPolicyRepositoryMock.Verify(
            x => x.GetActivePolicyByCategoryAndLocationAsync(
                query.CategoryCode,
                locationCode,
                It.IsAny<CancellationToken>()),
            Times.Once);

        // Should NOT call general pricing when location-specific pricing is found
        pricingPolicyRepositoryMock.Verify(
            x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithLocationCodeButNoLocationPolicy_ShouldFallbackToGeneralPricing()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        var locationCode = LocationCode.From("BER-HBF");

        var query = new CalculatePriceQuery(
            CategoryCode.From("SUV"),
            pickupDate,
            returnDate,
            locationCode);

        // No location-specific pricing
        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAndLocationAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BuildingBlocks.Domain.Exceptions.EntityNotFoundException(typeof(PricingPolicy), "location-specific"));

        // General pricing available
        var generalPolicy = PricingPolicyBuilder.Default()
            .AsSuv()
            .WithDailyRate(70.00m)
            .Build();

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(generalPolicy);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.TotalPriceNet.ShouldBe(210.00m); // 3 days * 70.00

        pricingPolicyRepositoryMock.Verify(
            x => x.GetActivePolicyByCategoryAndLocationAsync(
                query.CategoryCode,
                locationCode,
                It.IsAny<CancellationToken>()),
            Times.Once);

        pricingPolicyRepositoryMock.Verify(
            x => x.GetActivePolicyByCategoryAsync(
                query.CategoryCode,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNoPricingPolicy_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var query = new CalculatePriceQuery(
            CategoryCode.From("UNKNOWN"),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            null);

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BuildingBlocks.Domain.Exceptions.EntityNotFoundException(typeof(PricingPolicy), "UNKNOWN"));

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.HandleAsync(query, CancellationToken.None));

        ex.Message.ShouldContain("No active pricing policy found");
        ex.Message.ShouldContain("UNKNOWN");
    }

    [Fact]
    public async Task HandleAsync_WithDifferentCategories_ShouldCalculateCorrectly()
    {
        // Arrange
        var categories = new[]
        {
            ("KLEIN", 40.00m),
            ("KOMPAKT", 50.00m),
            ("MITTEL", 60.00m),
            ("OBER", 80.00m),
            ("SUV", 90.00m),
            ("LUXUS", 150.00m)
        };

        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)); // 3 days

        foreach (var (categoryCode, dailyRate) in categories)
        {
            var query = new CalculatePriceQuery(
                CategoryCode.From(categoryCode),
                pickupDate,
                returnDate, null);

            var policy = PricingPolicyBuilder.Default()
                .WithCategory(categoryCode)
                .WithDailyRate(dailyRate)
                .Build();

            pricingPolicyRepositoryMock
                .Setup(x => x.GetActivePolicyByCategoryAsync(
                    CategoryCode.From(categoryCode),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(policy);

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            result.CategoryCode.ShouldBe(categoryCode);
            result.DailyRateNet.ShouldBe(dailyRate);
            result.TotalPriceNet.ShouldBe(dailyRate * 3);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateVATCorrectly()
    {
        // Arrange
        var query = new CalculatePriceQuery(
            CategoryCode.From("KLEIN"),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), // 2 days,
            null);

        var pricingPolicy = PricingPolicyBuilder.Default()
            .WithCategory("KLEIN")
            .WithDailyRate(100.00m) // Net price per day
            .Build();

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pricingPolicy);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert (2 days * 100 EUR = 200 EUR net, 19% VAT = 38 EUR, Gross = 238 EUR)
        result.TotalPriceNet.ShouldBe(200.00m);
        result.VatAmount.ShouldBe(38.00m);
        result.TotalPriceGross.ShouldBe(238.00m);
        result.VatRate.ShouldBe(0.19m);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var query = new CalculatePriceQuery(
            CategoryCode.From("KLEIN"),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            null);

        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            handler.HandleAsync(query, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithWeekRental_ShouldCalculate7Days()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)); // 7 days

        var query = new CalculatePriceQuery(
            CategoryCode.From("MITTEL"),
            pickupDate,
            returnDate,
            null);

        var pricingPolicy = PricingPolicyBuilder.Default()
            .AsMidSize()
            .Build();

        pricingPolicyRepositoryMock
            .Setup(x => x.GetActivePolicyByCategoryAsync(
                It.IsAny<CategoryCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pricingPolicy);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.TotalDays.ShouldBe(7);
        result.TotalPriceNet.ShouldBe(420.00m); // 7 days * 60.00
    }
}
