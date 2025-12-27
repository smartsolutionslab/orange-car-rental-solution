using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.InsurancePackage;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class InsurancePackageTests
{
    [Fact]
    public void Basic_HasCorrectProperties()
    {
        // Act
        var package = InsurancePackage.Basic;

        // Assert
        package.Type.ShouldBe(InsuranceType.Haftpflicht);
        package.Deductible.GrossAmount.ShouldBe(2500.00m);
        package.DailySurcharge.GrossAmount.ShouldBe(0);
        package.IncludesTheftProtection.ShouldBeFalse();
        package.IncludesGlassAndTires.ShouldBeFalse();
        package.IncludesPersonalAccident.ShouldBeFalse();
        package.IsZeroDeductible.ShouldBeFalse();
    }

    [Fact]
    public void Standard_HasCorrectProperties()
    {
        // Act
        var package = InsurancePackage.Standard;

        // Assert
        package.Type.ShouldBe(InsuranceType.Teilkasko);
        package.Deductible.GrossAmount.ShouldBe(1000.00m);
        package.DailySurcharge.GrossAmount.ShouldBe(8.00m);
        package.IncludesTheftProtection.ShouldBeTrue();
        package.IncludesGlassAndTires.ShouldBeFalse();
        package.IncludesPersonalAccident.ShouldBeFalse();
    }

    [Fact]
    public void Comfort_HasCorrectProperties()
    {
        // Act
        var package = InsurancePackage.Comfort;

        // Assert
        package.Type.ShouldBe(InsuranceType.Vollkasko);
        package.Deductible.GrossAmount.ShouldBe(500.00m);
        package.DailySurcharge.GrossAmount.ShouldBe(15.00m);
        package.IncludesTheftProtection.ShouldBeTrue();
        package.IncludesGlassAndTires.ShouldBeTrue();
        package.IncludesPersonalAccident.ShouldBeFalse();
    }

    [Fact]
    public void Premium_HasCorrectProperties()
    {
        // Act
        var package = InsurancePackage.Premium;

        // Assert
        package.Type.ShouldBe(InsuranceType.VollkaskoZeroDeductible);
        package.Deductible.GrossAmount.ShouldBe(0);
        package.DailySurcharge.GrossAmount.ShouldBe(25.00m);
        package.IncludesTheftProtection.ShouldBeTrue();
        package.IncludesGlassAndTires.ShouldBeTrue();
        package.IncludesPersonalAccident.ShouldBeTrue();
        package.IsZeroDeductible.ShouldBeTrue();
    }

    [Fact]
    public void CalculateCost_MultipliesByDays()
    {
        // Arrange
        var package = InsurancePackage.Comfort; // 15€/day

        // Act
        var cost = package.CalculateCost(7);

        // Assert - 7 days * 15€ = 105€
        cost.GrossAmount.ShouldBe(105.00m);
    }

    [Fact]
    public void CalculateCost_Basic_ReturnsZero()
    {
        // Arrange
        var package = InsurancePackage.Basic; // 0€/day (included)

        // Act
        var cost = package.CalculateCost(7);

        // Assert
        cost.GrossAmount.ShouldBe(0);
    }

    [Fact]
    public void CalculateCost_ZeroDays_ReturnsZero()
    {
        // Arrange
        var package = InsurancePackage.Premium;

        // Act
        var cost = package.CalculateCost(0);

        // Assert
        cost.GrossAmount.ShouldBe(0);
    }

    [Fact]
    public void CalculateCost_NegativeDays_ReturnsZero()
    {
        // Arrange
        var package = InsurancePackage.Premium;

        // Act
        var cost = package.CalculateCost(-5);

        // Assert
        cost.GrossAmount.ShouldBe(0);
    }

    [Fact]
    public void GetGermanDisplayName_ReturnsCorrectName()
    {
        // Assert
        InsurancePackage.Basic.GetGermanDisplayName().ShouldBe("Haftpflichtversicherung (Basis)");
        InsurancePackage.Standard.GetGermanDisplayName().ShouldBe("Teilkasko (SB 1.000€)");
        InsurancePackage.Comfort.GetGermanDisplayName().ShouldBe("Vollkasko (SB 500€)");
        InsurancePackage.Premium.GetGermanDisplayName().ShouldBe("Vollkasko ohne Selbstbeteiligung");
    }

    [Fact]
    public void GetEnglishDisplayName_ReturnsCorrectName()
    {
        // Assert
        InsurancePackage.Basic.GetEnglishDisplayName().ShouldBe("Liability Only (Basic)");
        InsurancePackage.Standard.GetEnglishDisplayName().ShouldBe("Partial Coverage (€1,000 excess)");
        InsurancePackage.Comfort.GetEnglishDisplayName().ShouldBe("Comprehensive (€500 excess)");
        InsurancePackage.Premium.GetEnglishDisplayName().ShouldBe("Comprehensive Zero Excess");
    }

    [Fact]
    public void GetCoverageDescription_ReturnsNonEmptyDescription()
    {
        // Assert
        foreach (var package in InsurancePackage.GetAllPackages())
        {
            package.GetCoverageDescription().ShouldNotBeNullOrEmpty();
            package.GetGermanCoverageDescription().ShouldNotBeNullOrEmpty();
        }
    }

    [Fact]
    public void GetAllPackages_Returns4Packages()
    {
        // Act
        var packages = InsurancePackage.GetAllPackages();

        // Assert
        packages.Count.ShouldBe(4);
    }

    [Fact]
    public void FromType_ReturnsCorrectPackage()
    {
        // Act & Assert
        InsurancePackage.FromType(InsuranceType.Haftpflicht).ShouldBe(InsurancePackage.Basic);
        InsurancePackage.FromType(InsuranceType.Teilkasko).ShouldBe(InsurancePackage.Standard);
        InsurancePackage.FromType(InsuranceType.Vollkasko).ShouldBe(InsurancePackage.Comfort);
        InsurancePackage.FromType(InsuranceType.VollkaskoZeroDeductible).ShouldBe(InsurancePackage.Premium);
    }

    [Fact]
    public void FromType_InvalidType_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => InsurancePackage.FromType((InsuranceType)999));
    }

    [Theory]
    [InlineData(InsuranceType.Haftpflicht, false)]
    [InlineData(InsuranceType.Teilkasko, true)]
    [InlineData(InsuranceType.Vollkasko, true)]
    [InlineData(InsuranceType.VollkaskoZeroDeductible, true)]
    public void IncludesTheftProtection_ReturnsCorrectValue(InsuranceType type, bool expected)
    {
        // Act
        var package = InsurancePackage.FromType(type);

        // Assert
        package.IncludesTheftProtection.ShouldBe(expected);
    }

    [Theory]
    [InlineData(InsuranceType.Haftpflicht, false)]
    [InlineData(InsuranceType.Teilkasko, false)]
    [InlineData(InsuranceType.Vollkasko, true)]
    [InlineData(InsuranceType.VollkaskoZeroDeductible, true)]
    public void IncludesGlassAndTires_ReturnsCorrectValue(InsuranceType type, bool expected)
    {
        // Act
        var package = InsurancePackage.FromType(type);

        // Assert
        package.IncludesGlassAndTires.ShouldBe(expected);
    }

    [Theory]
    [InlineData(InsuranceType.Haftpflicht, false)]
    [InlineData(InsuranceType.Teilkasko, false)]
    [InlineData(InsuranceType.Vollkasko, false)]
    [InlineData(InsuranceType.VollkaskoZeroDeductible, true)]
    public void IncludesPersonalAccident_ReturnsCorrectValue(InsuranceType type, bool expected)
    {
        // Act
        var package = InsurancePackage.FromType(type);

        // Assert
        package.IncludesPersonalAccident.ShouldBe(expected);
    }

    [Fact]
    public void CompareCoverageTo_HigherTypeIsBetter()
    {
        // Act & Assert
        InsurancePackage.Basic.CompareCoverageTo(InsurancePackage.Standard).ShouldBeLessThan(0);
        InsurancePackage.Standard.CompareCoverageTo(InsurancePackage.Comfort).ShouldBeLessThan(0);
        InsurancePackage.Comfort.CompareCoverageTo(InsurancePackage.Premium).ShouldBeLessThan(0);
        InsurancePackage.Premium.CompareCoverageTo(InsurancePackage.Basic).ShouldBeGreaterThan(0);
        InsurancePackage.Standard.CompareCoverageTo(InsurancePackage.Standard).ShouldBe(0);
    }

    [Fact]
    public void CreateCustom_CreatesPackageWithCustomDeductible()
    {
        // Act
        var custom = InsurancePackage.CreateCustom(
            InsuranceType.Vollkasko,
            SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects.Money.EuroGross(750.00m),
            SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects.Money.EuroGross(12.00m));

        // Assert
        custom.Type.ShouldBe(InsuranceType.Vollkasko);
        custom.Deductible.GrossAmount.ShouldBe(750.00m);
        custom.DailySurcharge.GrossAmount.ShouldBe(12.00m);
        // Inherits coverage from base Vollkasko package
        custom.IncludesTheftProtection.ShouldBeTrue();
        custom.IncludesGlassAndTires.ShouldBeTrue();
    }

    [Fact]
    public void IsZeroDeductible_OnlyTrueForPremium()
    {
        // Assert
        InsurancePackage.Basic.IsZeroDeductible.ShouldBeFalse();
        InsurancePackage.Standard.IsZeroDeductible.ShouldBeFalse();
        InsurancePackage.Comfort.IsZeroDeductible.ShouldBeFalse();
        InsurancePackage.Premium.IsZeroDeductible.ShouldBeTrue();
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Act
        var result = InsurancePackage.Standard.ToString();

        // Assert
        result.ShouldContain("Partial Coverage");
        result.ShouldContain("/day");
    }

    [Fact]
    public void DeductibleAmounts_AreTypicalForGermanMarket()
    {
        // Verify typical German car rental deductible amounts
        InsurancePackage.Basic.Deductible.GrossAmount.ShouldBe(2500.00m); // High liability
        InsurancePackage.Standard.Deductible.GrossAmount.ShouldBe(1000.00m); // Standard
        InsurancePackage.Comfort.Deductible.GrossAmount.ShouldBe(500.00m); // Reduced
        InsurancePackage.Premium.Deductible.GrossAmount.ShouldBe(0m); // Zero
    }
}
