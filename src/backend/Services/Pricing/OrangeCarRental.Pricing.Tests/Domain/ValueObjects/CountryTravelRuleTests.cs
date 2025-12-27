using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class CountryTravelRuleTests
{
    [Fact]
    public void Allowed_WithoutSurcharge_HasCorrectProperties()
    {
        // Act
        var rule = CountryTravelRule.Allowed(CountryCode.Germany);

        // Assert
        rule.Country.ShouldBe(CountryCode.Germany);
        rule.Restriction.ShouldBe(TravelRestriction.Allowed);
        rule.DailySurcharge.ShouldBeNull();
        rule.RequiresAdditionalInsurance.ShouldBeFalse();
        rule.RestrictionReason.ShouldBeNull();
    }

    [Fact]
    public void Allowed_WithSurcharge_HasCorrectProperties()
    {
        // Arrange
        var surcharge = Money.EuroGross(5.00m);

        // Act
        var rule = CountryTravelRule.Allowed(CountryCode.Switzerland, surcharge);

        // Assert
        rule.Country.ShouldBe(CountryCode.Switzerland);
        rule.Restriction.ShouldBe(TravelRestriction.Allowed);
        rule.DailySurcharge.ShouldBe(surcharge);
        rule.RequiresAdditionalInsurance.ShouldBeFalse();
    }

    [Fact]
    public void RequiresPermit_HasCorrectProperties()
    {
        // Arrange
        var surcharge = Money.EuroGross(15.00m);

        // Act
        var rule = CountryTravelRule.RequiresPermit(CountryCode.Poland, surcharge);

        // Assert
        rule.Country.ShouldBe(CountryCode.Poland);
        rule.Restriction.ShouldBe(TravelRestriction.PermitRequired);
        rule.DailySurcharge.ShouldBe(surcharge);
        rule.RequiresAdditionalInsurance.ShouldBeFalse();
    }

    [Fact]
    public void RequiresPermit_WithAdditionalInsurance_HasCorrectProperties()
    {
        // Arrange
        var surcharge = Money.EuroGross(25.00m);

        // Act
        var rule = CountryTravelRule.RequiresPermit(CountryCode.Create("RO"), surcharge, true);

        // Assert
        rule.Restriction.ShouldBe(TravelRestriction.PermitRequired);
        rule.DailySurcharge.ShouldBe(surcharge);
        rule.RequiresAdditionalInsurance.ShouldBeTrue();
    }

    [Fact]
    public void Restricted_HasCorrectProperties()
    {
        // Act
        var rule = CountryTravelRule.Restricted(CountryCode.Create("GR"), "Ferry required");

        // Assert
        rule.Country.Value.ShouldBe("GR");
        rule.Restriction.ShouldBe(TravelRestriction.Restricted);
        rule.DailySurcharge.ShouldBeNull();
        rule.RequiresAdditionalInsurance.ShouldBeTrue();
        rule.RestrictionReason.ShouldBe("Ferry required");
    }

    [Fact]
    public void Restricted_WithoutAdditionalInsurance_HasCorrectProperties()
    {
        // Act
        var rule = CountryTravelRule.Restricted(CountryCode.Create("GR"), "Ferry required", false);

        // Assert
        rule.RequiresAdditionalInsurance.ShouldBeFalse();
    }

    [Fact]
    public void Prohibited_HasCorrectProperties()
    {
        // Act
        var rule = CountryTravelRule.Prohibited(CountryCode.Create("UA"), "Active conflict zone");

        // Assert
        rule.Country.Value.ShouldBe("UA");
        rule.Restriction.ShouldBe(TravelRestriction.Prohibited);
        rule.DailySurcharge.ShouldBeNull();
        rule.RequiresAdditionalInsurance.ShouldBeFalse();
        rule.RestrictionReason.ShouldBe("Active conflict zone");
    }

    [Fact]
    public void IsAllowed_AllowedRestriction_ReturnsTrue()
    {
        var rule = CountryTravelRule.Allowed(CountryCode.Germany);
        rule.IsAllowed.ShouldBeTrue();
    }

    [Fact]
    public void IsAllowed_PermitRequired_ReturnsTrue()
    {
        var rule = CountryTravelRule.RequiresPermit(CountryCode.Poland, Money.EuroGross(15.00m));
        rule.IsAllowed.ShouldBeTrue();
    }

    [Fact]
    public void IsAllowed_Restricted_ReturnsFalse()
    {
        var rule = CountryTravelRule.Restricted(CountryCode.Create("GR"), "Ferry required");
        rule.IsAllowed.ShouldBeFalse();
    }

    [Fact]
    public void IsAllowed_Prohibited_ReturnsFalse()
    {
        var rule = CountryTravelRule.Prohibited(CountryCode.Create("UA"), "Conflict zone");
        rule.IsAllowed.ShouldBeFalse();
    }

    [Fact]
    public void RequiresAdvanceBooking_PermitRequired_ReturnsTrue()
    {
        var rule = CountryTravelRule.RequiresPermit(CountryCode.Poland, Money.EuroGross(15.00m));
        rule.RequiresAdvanceBooking.ShouldBeTrue();
    }

    [Fact]
    public void RequiresAdvanceBooking_Allowed_ReturnsFalse()
    {
        var rule = CountryTravelRule.Allowed(CountryCode.Germany);
        rule.RequiresAdvanceBooking.ShouldBeFalse();
    }

    [Fact]
    public void GetGermanDescription_AllowedWithoutSurcharge_ReturnsCorrectText()
    {
        var rule = CountryTravelRule.Allowed(CountryCode.Germany);
        rule.GetGermanDescription().ShouldBe("Reisen nach Deutschland erlaubt");
    }

    [Fact]
    public void GetGermanDescription_AllowedWithSurcharge_IncludesSurcharge()
    {
        var rule = CountryTravelRule.Allowed(CountryCode.Switzerland, Money.EuroGross(5.00m));
        rule.GetGermanDescription().ShouldContain("Schweiz");
        rule.GetGermanDescription().ShouldContain("/Tag");
    }

    [Fact]
    public void GetGermanDescription_PermitRequired_IncludesPermitText()
    {
        var rule = CountryTravelRule.RequiresPermit(CountryCode.Poland, Money.EuroGross(15.00m));
        rule.GetGermanDescription().ShouldContain("Grenzübertrittserlaubnis");
        rule.GetGermanDescription().ShouldContain("Polen");
    }

    [Fact]
    public void GetGermanDescription_Prohibited_IncludesReason()
    {
        var rule = CountryTravelRule.Prohibited(CountryCode.Create("UA"), "Active conflict zone");
        rule.GetGermanDescription().ShouldContain("Ukraine");
        rule.GetGermanDescription().ShouldContain("nicht gestattet");
        rule.GetGermanDescription().ShouldContain("Active conflict zone");
    }

    [Fact]
    public void GetEnglishDescription_AllowedWithoutSurcharge_ReturnsCorrectText()
    {
        var rule = CountryTravelRule.Allowed(CountryCode.Germany);
        rule.GetEnglishDescription().ShouldBe("Travel to Germany allowed");
    }

    [Fact]
    public void GetEnglishDescription_AllowedWithSurcharge_IncludesSurcharge()
    {
        var rule = CountryTravelRule.Allowed(CountryCode.Switzerland, Money.EuroGross(5.00m));
        rule.GetEnglishDescription().ShouldContain("Switzerland");
        rule.GetEnglishDescription().ShouldContain("/day");
    }

    [Fact]
    public void GetEnglishDescription_PermitRequired_IncludesPermitText()
    {
        var rule = CountryTravelRule.RequiresPermit(CountryCode.Poland, Money.EuroGross(15.00m));
        rule.GetEnglishDescription().ShouldContain("cross-border permit");
        rule.GetEnglishDescription().ShouldContain("Poland");
    }

    [Fact]
    public void GetEnglishDescription_Restricted_IncludesReason()
    {
        var rule = CountryTravelRule.Restricted(CountryCode.Create("GR"), "Ferry required");
        rule.GetEnglishDescription().ShouldContain("Greece");
        rule.GetEnglishDescription().ShouldContain("special approval");
        rule.GetEnglishDescription().ShouldContain("Ferry required");
    }

    [Fact]
    public void GetEnglishDescription_Prohibited_IncludesReason()
    {
        var rule = CountryTravelRule.Prohibited(CountryCode.Create("RU"), "Sanctions");
        rule.GetEnglishDescription().ShouldContain("Russia");
        rule.GetEnglishDescription().ShouldContain("not permitted");
        rule.GetEnglishDescription().ShouldContain("Sanctions");
    }
}
