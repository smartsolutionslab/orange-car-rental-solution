using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class CrossBorderPolicyTests
{
    #region Standard German Policy Tests

    [Fact]
    public void StandardGermanPolicy_Germany_IsAllowed()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        policy.IsAllowed(CountryCode.Germany).ShouldBeTrue();
    }

    [Fact]
    public void StandardGermanPolicy_Austria_IsAllowed()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        policy.IsAllowed(CountryCode.Austria).ShouldBeTrue();
    }

    [Fact]
    public void StandardGermanPolicy_Switzerland_IsAllowedWithSurcharge()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        policy.IsAllowed(CountryCode.Switzerland).ShouldBeTrue();
        policy.GetDailySurcharge(CountryCode.Switzerland).ShouldNotBeNull();
    }

    [Fact]
    public void StandardGermanPolicy_Poland_RequiresPermit()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        policy.IsAllowed(CountryCode.Poland).ShouldBeTrue();
        policy.RequiresPermit(CountryCode.Poland).ShouldBeTrue();
    }

    [Fact]
    public void StandardGermanPolicy_Ukraine_IsProhibited()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var ukraine = CountryCode.Create("UA");
        policy.IsAllowed(ukraine).ShouldBeFalse();
    }

    [Fact]
    public void StandardGermanPolicy_Russia_IsProhibited()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var russia = CountryCode.Create("RU");
        policy.IsAllowed(russia).ShouldBeFalse();
    }

    [Fact]
    public void StandardGermanPolicy_Germany_NoSurcharge()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        policy.GetDailySurcharge(CountryCode.Germany).ShouldBeNull();
    }

    [Fact]
    public void StandardGermanPolicy_Germany_NoPermitRequired()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        policy.RequiresPermit(CountryCode.Germany).ShouldBeFalse();
    }

    #endregion

    #region Premium German Policy Tests

    [Fact]
    public void PremiumGermanPolicy_Poland_IsAllowedWithoutPermit()
    {
        var policy = CrossBorderPolicy.PremiumGermanPolicy;
        policy.IsAllowed(CountryCode.Poland).ShouldBeTrue();
        policy.RequiresPermit(CountryCode.Poland).ShouldBeFalse();
    }

    [Fact]
    public void PremiumGermanPolicy_Romania_RequiresPermit()
    {
        var policy = CrossBorderPolicy.PremiumGermanPolicy;
        var romania = CountryCode.Create("RO");
        policy.IsAllowed(romania).ShouldBeTrue();
        policy.RequiresPermit(romania).ShouldBeTrue();
    }

    [Fact]
    public void PremiumGermanPolicy_StillProhibitsUkraine()
    {
        var policy = CrossBorderPolicy.PremiumGermanPolicy;
        var ukraine = CountryCode.Create("UA");
        policy.IsAllowed(ukraine).ShouldBeFalse();
    }

    #endregion

    #region GetRule Tests

    [Fact]
    public void GetRule_ExistingCountry_ReturnsRule()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var rule = policy.GetRule(CountryCode.Germany);
        rule.ShouldNotBeNull();
        rule!.Country.ShouldBe(CountryCode.Germany);
    }

    [Fact]
    public void GetRule_UnknownCountry_ReturnsNull()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var rule = policy.GetRule(CountryCode.Create("XX"));
        rule.ShouldBeNull();
    }

    #endregion

    #region CalculateTotalSurcharge Tests

    [Fact]
    public void CalculateTotalSurcharge_NoCountries_ReturnsZero()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var total = policy.CalculateTotalSurcharge([], 5);
        total.GrossAmount.ShouldBe(0m);
    }

    [Fact]
    public void CalculateTotalSurcharge_CountryWithNoSurcharge_ReturnsZero()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var total = policy.CalculateTotalSurcharge([CountryCode.Germany], 5);
        total.GrossAmount.ShouldBe(0m);
    }

    [Fact]
    public void CalculateTotalSurcharge_SingleCountryWithSurcharge_CalculatesCorrectly()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        // Switzerland has 5€/day surcharge
        var total = policy.CalculateTotalSurcharge([CountryCode.Switzerland], 5);
        total.GrossAmount.ShouldBe(25.00m); // 5 * 5€
    }

    [Fact]
    public void CalculateTotalSurcharge_MultipleCountries_SumsCorrectly()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        // Switzerland: 5€/day, Poland: 15€/day
        var countries = new[] { CountryCode.Switzerland, CountryCode.Poland };
        var total = policy.CalculateTotalSurcharge(countries, 3);
        total.GrossAmount.ShouldBe(60.00m); // 3 * (5 + 15)€
    }

    [Fact]
    public void CalculateTotalSurcharge_DuplicateCountries_CountsOnce()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var countries = new[] { CountryCode.Switzerland, CountryCode.Switzerland };
        var total = policy.CalculateTotalSurcharge(countries, 5);
        total.GrossAmount.ShouldBe(25.00m); // Only counted once
    }

    #endregion

    #region Validate Tests

    [Fact]
    public void Validate_AllowedCountries_ReturnsValid()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var result = policy.Validate([CountryCode.Germany, CountryCode.Austria]);
        result.IsValid.ShouldBeTrue();
        result.Issues.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_PermitRequiredCountry_SetsRequiresPermit()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var result = policy.Validate([CountryCode.Poland]);
        result.IsValid.ShouldBeTrue();
        result.RequiresPermit.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ProhibitedCountry_ReturnsInvalid()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var ukraine = CountryCode.Create("UA");
        var result = policy.Validate([ukraine]);
        result.IsValid.ShouldBeFalse();
        result.Issues.ShouldContain(i => i.Contains("Ukraine") && i.Contains("not permitted"));
    }

    [Fact]
    public void Validate_RestrictedCountry_ReturnsInvalid()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var greece = CountryCode.Create("GR");
        var result = policy.Validate([greece]);
        result.IsValid.ShouldBeFalse();
        result.Issues.ShouldContain(i => i.Contains("Greece") && i.Contains("special approval"));
    }

    [Fact]
    public void Validate_UnknownCountry_ReturnsInvalid()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var unknown = CountryCode.Create("XX");
        var result = policy.Validate([unknown]);
        result.IsValid.ShouldBeFalse();
        result.Issues.ShouldContain(i => i.Contains("not covered"));
    }

    [Fact]
    public void Validate_CountryRequiringAdditionalInsurance_SetsFlag()
    {
        var policy = CrossBorderPolicy.PremiumGermanPolicy;
        // Romania in premium policy requires additional insurance
        var romania = CountryCode.Create("RO");
        var result = policy.Validate([romania]);
        result.RequiresAdditionalInsurance.ShouldBeTrue();
    }

    [Fact]
    public void Validate_MixedCountries_ReportsAllIssues()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var ukraine = CountryCode.Create("UA");
        var russia = CountryCode.Create("RU");
        var result = policy.Validate([ukraine, russia]);
        result.IsValid.ShouldBeFalse();
        result.Issues.Count.ShouldBe(2);
    }

    #endregion

    #region GetAllowedCountries Tests

    [Fact]
    public void GetAllowedCountries_StandardPolicy_ContainsGermany()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var allowed = policy.GetAllowedCountries();
        allowed.ShouldContain(CountryCode.Germany);
    }

    [Fact]
    public void GetAllowedCountries_StandardPolicy_ContainsPoland()
    {
        // Poland requires permit but is still "allowed"
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var allowed = policy.GetAllowedCountries();
        allowed.ShouldContain(CountryCode.Poland);
    }

    [Fact]
    public void GetAllowedCountries_StandardPolicy_DoesNotContainUkraine()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var allowed = policy.GetAllowedCountries();
        allowed.ShouldNotContain(c => c.Value == "UA");
    }

    #endregion

    #region GetPermitRequiredCountries Tests

    [Fact]
    public void GetPermitRequiredCountries_StandardPolicy_ContainsPoland()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var permitRequired = policy.GetPermitRequiredCountries();
        permitRequired.ShouldContain(CountryCode.Poland);
    }

    [Fact]
    public void GetPermitRequiredCountries_StandardPolicy_DoesNotContainAustria()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var permitRequired = policy.GetPermitRequiredCountries();
        permitRequired.ShouldNotContain(CountryCode.Austria);
    }

    [Fact]
    public void GetPermitRequiredCountries_PremiumPolicy_DoesNotContainPoland()
    {
        // Premium policy allows Poland without permit
        var policy = CrossBorderPolicy.PremiumGermanPolicy;
        var permitRequired = policy.GetPermitRequiredCountries();
        permitRequired.ShouldNotContain(CountryCode.Poland);
    }

    #endregion

    #region GetProhibitedCountries Tests

    [Fact]
    public void GetProhibitedCountries_StandardPolicy_ContainsUkraine()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var prohibited = policy.GetProhibitedCountries();
        prohibited.ShouldContain(c => c.Value == "UA");
    }

    [Fact]
    public void GetProhibitedCountries_StandardPolicy_ContainsRussia()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var prohibited = policy.GetProhibitedCountries();
        prohibited.ShouldContain(c => c.Value == "RU");
    }

    [Fact]
    public void GetProhibitedCountries_StandardPolicy_ContainsBelarus()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var prohibited = policy.GetProhibitedCountries();
        prohibited.ShouldContain(c => c.Value == "BY");
    }

    [Fact]
    public void GetProhibitedCountries_StandardPolicy_DoesNotContainGermany()
    {
        var policy = CrossBorderPolicy.StandardGermanPolicy;
        var prohibited = policy.GetProhibitedCountries();
        prohibited.ShouldNotContain(CountryCode.Germany);
    }

    #endregion

    #region Create Custom Policy Tests

    [Fact]
    public void Create_CustomPolicy_Works()
    {
        // Arrange
        var rules = new[]
        {
            CountryTravelRule.Allowed(CountryCode.Germany),
            CountryTravelRule.Allowed(CountryCode.Austria),
            CountryTravelRule.Prohibited(CountryCode.Create("XX"), "Custom reason")
        };

        // Act
        var policy = CrossBorderPolicy.Create(rules);

        // Assert
        policy.IsAllowed(CountryCode.Germany).ShouldBeTrue();
        policy.IsAllowed(CountryCode.Austria).ShouldBeTrue();
        policy.IsAllowed(CountryCode.Create("XX")).ShouldBeFalse();
    }

    #endregion

    #region CrossBorderValidationResult Tests

    [Fact]
    public void CrossBorderValidationResult_Valid_HasCorrectDefaults()
    {
        var result = CrossBorderValidationResult.Valid();
        result.IsValid.ShouldBeTrue();
        result.Issues.ShouldBeEmpty();
        result.RequiresPermit.ShouldBeFalse();
        result.RequiresAdditionalInsurance.ShouldBeFalse();
    }

    [Fact]
    public void CrossBorderValidationResult_ValidWithPermit_HasCorrectProperties()
    {
        var result = CrossBorderValidationResult.Valid(requiresPermit: true);
        result.IsValid.ShouldBeTrue();
        result.RequiresPermit.ShouldBeTrue();
    }

    [Fact]
    public void CrossBorderValidationResult_ValidWithAdditionalInsurance_HasCorrectProperties()
    {
        var result = CrossBorderValidationResult.Valid(requiresAdditionalInsurance: true);
        result.IsValid.ShouldBeTrue();
        result.RequiresAdditionalInsurance.ShouldBeTrue();
    }

    #endregion
}
