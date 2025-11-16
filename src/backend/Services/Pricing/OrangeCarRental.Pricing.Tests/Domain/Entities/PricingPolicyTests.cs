using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.Entities;

public class PricingPolicyTests
{
    private readonly CategoryCode validCategory = CategoryCode.Of("KLEIN");
    private readonly Money validDailyRate = Money.Euro(49.99m);

    [Fact]
    public void Create_WithValidData_ShouldCreatePricingPolicy()
    {
        // Act
        var policy = PricingPolicy.Create(validCategory, validDailyRate);

        // Assert
        policy.ShouldNotBeNull();
        policy.Id.Value.ShouldNotBe(Guid.Empty);
        policy.CategoryCode.ShouldBe(validCategory);
        policy.DailyRate.ShouldBe(validDailyRate);
        policy.IsActive.ShouldBeTrue();
        policy.EffectiveFrom.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
        policy.EffectiveUntil.ShouldBeNull();
        policy.LocationCode.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEffectiveDates_ShouldSetDatesCorrectly()
    {
        // Arrange
        var effectiveFrom = DateTime.UtcNow.AddDays(1);
        var effectiveUntil = DateTime.UtcNow.AddDays(30);

        // Act
        var policy = PricingPolicy.Create(validCategory, validDailyRate, effectiveFrom, effectiveUntil);

        // Assert
        policy.EffectiveFrom.ShouldBe(effectiveFrom);
        policy.EffectiveUntil.ShouldBe(effectiveUntil);
    }

    [Fact]
    public void Create_WithLocationCode_ShouldSetLocationCodeCorrectly()
    {
        // Arrange
        var locationCode = LocationCode.Of("BER-HBF");

        // Act
        var policy = PricingPolicy.Create(validCategory, validDailyRate, locationCode: locationCode);

        // Assert
        policy.LocationCode.ShouldBe(locationCode);
    }

    [Fact]
    public void Create_ShouldRaisePricingPolicyCreatedEvent()
    {
        // Act
        var policy = PricingPolicy.Create(validCategory, validDailyRate);

        // Assert
        var events = policy.DomainEvents;
        events.ShouldNotBeEmpty();
        var createdEvent = events.ShouldHaveSingleItem();
        createdEvent.ShouldBeOfType<PricingPolicyCreated>();

        var evt = (PricingPolicyCreated)createdEvent;
        evt.PricingPolicyId.ShouldBe(policy.Id);
        evt.CategoryCode.ShouldBe(validCategory);
        evt.DailyRate.ShouldBe(validDailyRate);
        evt.EffectiveFrom.ShouldBe(policy.EffectiveFrom);
    }

    [Fact]
    public void UpdateDailyRate_WithDifferentRate_ShouldReturnNewInstance()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);
        var newRate = Money.Euro(59.99m);

        // Act
        var updatedPolicy = policy.UpdateDailyRate(newRate);

        // Assert
        updatedPolicy.ShouldNotBeSameAs(policy); // New instance (immutable)
        updatedPolicy.Id.ShouldBe(policy.Id); // Same ID
        updatedPolicy.DailyRate.ShouldBe(newRate);
        policy.DailyRate.ShouldBe(validDailyRate); // Original unchanged
    }

    [Fact]
    public void UpdateDailyRate_WithSameRate_ShouldReturnSameInstance()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);

        // Act
        var updatedPolicy = policy.UpdateDailyRate(validDailyRate);

        // Assert
        updatedPolicy.ShouldBeSameAs(policy); // Same instance if no change
    }

    [Fact]
    public void UpdateDailyRate_ShouldRaisePricingPolicyUpdatedEvent()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);
        policy.ClearDomainEvents(); // Clear creation event
        var newRate = Money.Euro(59.99m);

        // Act
        var updatedPolicy = policy.UpdateDailyRate(newRate);

        // Assert
        var events = updatedPolicy.DomainEvents;
        events.ShouldNotBeEmpty();
        var updatedEvent = events.ShouldHaveSingleItem();
        updatedEvent.ShouldBeOfType<PricingPolicyUpdated>();

        var evt = (PricingPolicyUpdated)updatedEvent;
        evt.PricingPolicyId.ShouldBe(policy.Id);
        evt.CategoryCode.ShouldBe(validCategory);
        evt.OldDailyRate.ShouldBe(validDailyRate);
        evt.NewDailyRate.ShouldBe(newRate);
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldReturnDeactivatedInstance()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);

        // Act
        var deactivatedPolicy = policy.Deactivate();

        // Assert
        deactivatedPolicy.ShouldNotBeSameAs(policy); // New instance (immutable)
        deactivatedPolicy.Id.ShouldBe(policy.Id); // Same ID
        deactivatedPolicy.IsActive.ShouldBeFalse();
        deactivatedPolicy.EffectiveUntil.ShouldNotBeNull();
        deactivatedPolicy.EffectiveUntil.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
        policy.IsActive.ShouldBeTrue(); // Original unchanged
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldReturnSameInstance()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);
        var deactivatedPolicy = policy.Deactivate();

        // Act
        var secondDeactivation = deactivatedPolicy.Deactivate();

        // Assert
        secondDeactivation.ShouldBeSameAs(deactivatedPolicy); // Same instance, no change
    }

    [Fact]
    public void Deactivate_ShouldRaisePricingPolicyDeactivatedEvent()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);
        policy.ClearDomainEvents(); // Clear creation event

        // Act
        var deactivatedPolicy = policy.Deactivate();

        // Assert
        var events = deactivatedPolicy.DomainEvents;
        events.ShouldNotBeEmpty();
        var deactivatedEvent = events.ShouldHaveSingleItem();
        deactivatedEvent.ShouldBeOfType<PricingPolicyDeactivated>();

        var evt = (PricingPolicyDeactivated)deactivatedEvent;
        evt.PricingPolicyId.ShouldBe(policy.Id);
        evt.CategoryCode.ShouldBe(validCategory);
    }

    [Fact]
    public void IsValidFor_WhenActiveAndWithinEffectiveDates_ShouldReturnTrue()
    {
        // Arrange
        var effectiveFrom = DateTime.UtcNow.AddDays(-10);
        var effectiveUntil = DateTime.UtcNow.AddDays(10);
        var policy = PricingPolicy.Create(validCategory, validDailyRate, effectiveFrom, effectiveUntil);

        // Act
        var isValid = policy.IsValidFor(DateTime.UtcNow);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsValidFor_WhenInactive_ShouldReturnFalse()
    {
        // Arrange
        var policy = PricingPolicy.Create(validCategory, validDailyRate);
        var deactivatedPolicy = policy.Deactivate();

        // Act
        var isValid = deactivatedPolicy.IsValidFor(DateTime.UtcNow);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsValidFor_WhenDateBeforeEffectiveFrom_ShouldReturnFalse()
    {
        // Arrange
        var effectiveFrom = DateTime.UtcNow.AddDays(10);
        var policy = PricingPolicy.Create(validCategory, validDailyRate, effectiveFrom);

        // Act
        var isValid = policy.IsValidFor(DateTime.UtcNow);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsValidFor_WhenDateAfterEffectiveUntil_ShouldReturnFalse()
    {
        // Arrange
        var effectiveFrom = DateTime.UtcNow.AddDays(-20);
        var effectiveUntil = DateTime.UtcNow.AddDays(-10);
        var policy = PricingPolicy.Create(validCategory, validDailyRate, effectiveFrom, effectiveUntil);

        // Act
        var isValid = policy.IsValidFor(DateTime.UtcNow);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsValidFor_WhenNoEffectiveUntil_ShouldReturnTrue()
    {
        // Arrange
        var effectiveFrom = DateTime.UtcNow.AddDays(-10);
        var policy = PricingPolicy.Create(validCategory, validDailyRate, effectiveFrom);

        // Act
        var isValid = policy.IsValidFor(DateTime.UtcNow.AddDays(100));

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void CalculatePrice_WithValidPeriod_ShouldCalculateCorrectly()
    {
        // Arrange
        var dailyRate = Money.Euro(50.00m);
        var policy = PricingPolicy.Create(validCategory, dailyRate);

        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(3);
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Act
        var totalPrice = policy.CalculatePrice(period);

        // Assert (4 days * 50.00 EUR = 200.00 EUR net, 238.00 EUR gross with 19% VAT)
        totalPrice.NetAmount.ShouldBe(200.00m);
        totalPrice.GrossAmount.ShouldBe(238.00m);
        totalPrice.Currency.ShouldBe(Currency.EUR);
    }

    [Fact]
    public void CalculatePrice_WithOneDayRental_ShouldCalculateCorrectly()
    {
        // Arrange
        var dailyRate = Money.Euro(50.00m);
        var policy = PricingPolicy.Create(validCategory, dailyRate);

        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(1);
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Act
        var totalPrice = policy.CalculatePrice(period);

        // Assert (2 days * 50.00 EUR = 100.00 EUR net, 119.00 EUR gross with 19% VAT)
        totalPrice.NetAmount.ShouldBe(100.00m);
        totalPrice.GrossAmount.ShouldBe(119.00m);
    }

    [Fact]
    public void CalculatePrice_WhenPolicyNotValidForDate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var effectiveFrom = DateTime.UtcNow.AddDays(10);
        var effectiveUntil = DateTime.UtcNow.AddDays(20);
        var policy = PricingPolicy.Create(validCategory, validDailyRate, effectiveFrom, effectiveUntil);

        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)); // Before effective from
        var returnDate = pickupDate.AddDays(3);
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => policy.CalculatePrice(period));
        ex.Message.ShouldContain("not valid for the requested pickup date");
    }
}
