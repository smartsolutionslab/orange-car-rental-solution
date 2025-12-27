using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.KilometerPackage;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class KilometerPackageTests
{
    [Fact]
    public void Limited100_HasCorrectProperties()
    {
        // Act
        var package = KilometerPackage.Limited100;

        // Assert
        package.Type.ShouldBe(KilometerPackageType.Limited100);
        package.DailyLimitKm.ShouldBe(100);
        package.AdditionalKmRate.ShouldNotBeNull();
        package.AdditionalKmRate!.Value.GrossAmount.ShouldBe(0.20m);
        package.IsUnlimited.ShouldBeFalse();
    }

    [Fact]
    public void Limited200_HasCorrectProperties()
    {
        // Act
        var package = KilometerPackage.Limited200;

        // Assert
        package.Type.ShouldBe(KilometerPackageType.Limited200);
        package.DailyLimitKm.ShouldBe(200);
        package.AdditionalKmRate.ShouldNotBeNull();
        package.IsUnlimited.ShouldBeFalse();
    }

    [Fact]
    public void Unlimited_HasCorrectProperties()
    {
        // Act
        var package = KilometerPackage.Unlimited;

        // Assert
        package.Type.ShouldBe(KilometerPackageType.Unlimited);
        package.DailyLimitKm.ShouldBeNull();
        package.AdditionalKmRate.ShouldBeNull();
        package.IsUnlimited.ShouldBeTrue();
    }

    [Fact]
    public void GetTotalAllowance_Limited100For5Days_Returns500()
    {
        // Arrange
        var package = KilometerPackage.Limited100;

        // Act
        var allowance = package.GetTotalAllowance(5);

        // Assert
        allowance.ShouldBe(500);
    }

    [Fact]
    public void GetTotalAllowance_Unlimited_ReturnsNull()
    {
        // Arrange
        var package = KilometerPackage.Unlimited;

        // Act
        var allowance = package.GetTotalAllowance(5);

        // Assert
        allowance.ShouldBeNull();
    }

    [Fact]
    public void CalculateAdditionalCharge_WithinLimit_ReturnsZero()
    {
        // Arrange
        var package = KilometerPackage.Limited100;

        // Act - 5 days * 100 km = 500 km allowance, drove 400 km
        var charge = package.CalculateAdditionalCharge(5, 400);

        // Assert
        charge.GrossAmount.ShouldBe(0);
    }

    [Fact]
    public void CalculateAdditionalCharge_ExceedsLimit_CalculatesCorrectly()
    {
        // Arrange
        var package = KilometerPackage.Limited100;

        // Act - 5 days * 100 km = 500 km allowance, drove 600 km = 100 km excess
        var charge = package.CalculateAdditionalCharge(5, 600);

        // Assert - 100 km * 0.20€ = 20€
        charge.GrossAmount.ShouldBe(20.00m);
    }

    [Fact]
    public void CalculateAdditionalCharge_Unlimited_ReturnsZero()
    {
        // Arrange
        var package = KilometerPackage.Unlimited;

        // Act
        var charge = package.CalculateAdditionalCharge(5, 10000);

        // Assert
        charge.GrossAmount.ShouldBe(0);
    }

    [Fact]
    public void FromType_ReturnsCorrectPackage()
    {
        // Act & Assert
        KilometerPackage.FromType(KilometerPackageType.Limited100).ShouldBe(KilometerPackage.Limited100);
        KilometerPackage.FromType(KilometerPackageType.Limited200).ShouldBe(KilometerPackage.Limited200);
        KilometerPackage.FromType(KilometerPackageType.Unlimited).ShouldBe(KilometerPackage.Unlimited);
    }

    [Fact]
    public void GetGermanDisplayName_ReturnsCorrectName()
    {
        // Assert
        KilometerPackage.Limited100.GetGermanDisplayName().ShouldBe("100 km/Tag inklusive");
        KilometerPackage.Limited200.GetGermanDisplayName().ShouldBe("200 km/Tag inklusive");
        KilometerPackage.Unlimited.GetGermanDisplayName().ShouldBe("Unbegrenzte Kilometer");
    }

    [Fact]
    public void GetEnglishDisplayName_ReturnsCorrectName()
    {
        // Assert
        KilometerPackage.Limited100.GetEnglishDisplayName().ShouldBe("100 km/day included");
        KilometerPackage.Limited200.GetEnglishDisplayName().ShouldBe("200 km/day included");
        KilometerPackage.Unlimited.GetEnglishDisplayName().ShouldBe("Unlimited kilometers");
    }
}
