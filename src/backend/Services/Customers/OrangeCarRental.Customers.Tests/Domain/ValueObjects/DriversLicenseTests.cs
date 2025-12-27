using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class DriversLicenseTests
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    private static DateOnly TwoYearsAgo => Today.AddYears(-2);
    private static DateOnly FiveYearsFromNow => Today.AddYears(5);

    [Fact]
    public void Of_ValidData_CreatesDriversLicense()
    {
        // Act
        var license = DriversLicense.Of(
            "ABC12345",
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow);

        // Assert
        license.LicenseNumber.ShouldBe("ABC12345");
        license.IssueCountry.ShouldBe("Germany");
        license.IssueDate.ShouldBe(TwoYearsAgo);
        license.ExpiryDate.ShouldBe(FiveYearsFromNow);
    }

    [Fact]
    public void Of_NormalizesLicenseNumber_ToUppercase()
    {
        // Act
        var license = DriversLicense.Of(
            "abc12345",
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow);

        // Assert
        license.LicenseNumber.ShouldBe("ABC12345");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Of_InvalidLicenseNumber_ThrowsArgumentException(string? invalidNumber)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => DriversLicense.Of(
            invalidNumber!,
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow));
    }

    [Fact]
    public void Of_LicenseNumberTooShort_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => DriversLicense.Of(
            "AB12",
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow));
    }

    [Fact]
    public void Of_ExpiryDateBeforeIssueDate_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => DriversLicense.Of(
            "ABC12345",
            "Germany",
            Today,
            Today.AddDays(-1)));
    }

    [Fact]
    public void Of_IssueDateInFuture_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => DriversLicense.Of(
            "ABC12345",
            "Germany",
            Today.AddDays(1),
            FiveYearsFromNow));
    }

    [Fact]
    public void IsValid_NotExpired_ReturnsTrue()
    {
        // Arrange
        var license = DriversLicense.Of(
            "ABC12345",
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow);

        // Assert
        license.IsValid().ShouldBeTrue();
    }

    [Fact]
    public void IsValid_Expired_ReturnsFalse()
    {
        // Arrange
        var license = DriversLicense.Of(
            "ABC12345",
            "Germany",
            Today.AddYears(-5),
            Today.AddDays(-1));

        // Assert
        license.IsValid().ShouldBeFalse();
    }

    [Fact]
    public void IsValidOn_DateWithinRange_ReturnsTrue()
    {
        // Arrange
        var license = DriversLicense.Of(
            "ABC12345",
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow);

        // Assert
        license.IsValidOn(Today.AddYears(1)).ShouldBeTrue();
    }

    [Fact]
    public void IsValidOn_DateAfterExpiry_ReturnsFalse()
    {
        // Arrange
        var license = DriversLicense.Of(
            "ABC12345",
            "Germany",
            TwoYearsAgo,
            FiveYearsFromNow);

        // Assert
        license.IsValidOn(FiveYearsFromNow.AddDays(1)).ShouldBeFalse();
    }

    [Theory]
    [InlineData("Germany")]
    [InlineData("Deutschland")]
    [InlineData("France")]
    [InlineData("Austria")]
    [InlineData("Netherlands")]
    public void IsEuLicense_EuCountry_ReturnsTrue(string country)
    {
        // Arrange
        var license = DriversLicense.Of("ABC12345", country, TwoYearsAgo, FiveYearsFromNow);

        // Assert
        license.IsEuLicense().ShouldBeTrue();
    }

    [Theory]
    [InlineData("USA")]
    [InlineData("Canada")]
    [InlineData("Japan")]
    [InlineData("Australia")]
    public void IsEuLicense_NonEuCountry_ReturnsFalse(string country)
    {
        // Arrange
        var license = DriversLicense.Of("ABC12345", country, TwoYearsAgo, FiveYearsFromNow);

        // Assert
        license.IsEuLicense().ShouldBeFalse();
    }

    [Fact]
    public void DaysUntilExpiry_ReturnsCorrectDays()
    {
        // Arrange
        var expiryDate = Today.AddDays(100);
        var license = DriversLicense.Of("ABC12345", "Germany", TwoYearsAgo, expiryDate);

        // Assert
        license.DaysUntilExpiry().ShouldBe(100);
    }

    [Fact]
    public void DaysUntilExpiry_Expired_ReturnsNegative()
    {
        // Arrange
        var expiryDate = Today.AddDays(-10);
        var license = DriversLicense.Of("ABC12345", "Germany", Today.AddYears(-5), expiryDate);

        // Assert
        license.DaysUntilExpiry().ShouldBe(-10);
    }

    [Fact]
    public void Anonymized_CreatesAnonymizedLicense()
    {
        // Act
        var license = DriversLicense.Anonymized();

        // Assert
        license.LicenseNumber.ShouldBe("ANONYMIZED000000");
        license.IssueCountry.ShouldBe("Germany");
    }

    // German Rental Requirements Tests

    [Fact]
    public void HasBeenHeldForYears_LicenseHeldLongEnough_ReturnsTrue()
    {
        // Arrange
        var license = DriversLicense.Of("ABC12345", "Germany", TwoYearsAgo, FiveYearsFromNow);

        // Assert
        license.HasBeenHeldForYears(1).ShouldBeTrue();
        license.HasBeenHeldForYears(2).ShouldBeTrue();
    }

    [Fact]
    public void HasBeenHeldForYears_LicenseNotHeldLongEnough_ReturnsFalse()
    {
        // Arrange
        var sixMonthsAgo = Today.AddMonths(-6);
        var license = DriversLicense.Of("ABC12345", "Germany", sixMonthsAgo, FiveYearsFromNow);

        // Assert
        license.HasBeenHeldForYears(1).ShouldBeFalse();
    }

    [Fact]
    public void HasBeenHeldForYears_WithSpecificDate_CalculatesCorrectly()
    {
        // Arrange
        var issueDate = new DateOnly(2023, 1, 1);
        var license = DriversLicense.Of("ABC12345", "Germany", issueDate, FiveYearsFromNow);
        var checkDate = new DateOnly(2024, 6, 1); // 1 year 5 months later

        // Assert
        license.HasBeenHeldForYears(1, checkDate).ShouldBeTrue();
        license.HasBeenHeldForYears(2, checkDate).ShouldBeFalse();
    }

    [Fact]
    public void YearsHeld_ReturnsCompleteYears()
    {
        // Arrange
        var twoAndHalfYearsAgo = Today.AddYears(-2).AddMonths(-6);
        var license = DriversLicense.Of("ABC12345", "Germany", twoAndHalfYearsAgo, FiveYearsFromNow);

        // Assert
        license.YearsHeld().ShouldBe(2);
    }

    [Fact]
    public void IsValidForGermanRental_AllRequirementsMet_ReturnsTrue()
    {
        // Arrange
        var license = DriversLicense.Of("ABC12345", "Germany", TwoYearsAgo, FiveYearsFromNow);
        var rentalDate = Today.AddDays(30);

        // Assert
        license.IsValidForGermanRental(rentalDate).ShouldBeTrue();
    }

    [Fact]
    public void IsValidForGermanRental_LicenseExpiresBeforeRental_ReturnsFalse()
    {
        // Arrange
        var expiryDate = Today.AddDays(10);
        var license = DriversLicense.Of("ABC12345", "Germany", TwoYearsAgo, expiryDate);
        var rentalDate = Today.AddDays(20);

        // Assert
        license.IsValidForGermanRental(rentalDate).ShouldBeFalse();
    }

    [Fact]
    public void IsValidForGermanRental_LicenseNotHeldForOneYear_ReturnsFalse()
    {
        // Arrange
        var sixMonthsAgo = Today.AddMonths(-6);
        var license = DriversLicense.Of("ABC12345", "Germany", sixMonthsAgo, FiveYearsFromNow);

        // Assert
        license.IsValidForGermanRental(Today).ShouldBeFalse();
    }

    [Fact]
    public void ValidateForGermanRental_AllRequirementsMet_ReturnsValid()
    {
        // Arrange
        var license = DriversLicense.Of("ABC12345", "Germany", TwoYearsAgo, FiveYearsFromNow);

        // Act
        var result = license.ValidateForGermanRental(Today);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Issues.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateForGermanRental_ExpiredLicense_ReturnsIssue()
    {
        // Arrange
        var expiryDate = Today.AddDays(-10);
        var license = DriversLicense.Of("ABC12345", "Germany", Today.AddYears(-5), expiryDate);

        // Act
        var result = license.ValidateForGermanRental(Today);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Issues.ShouldContain(i => i.Contains("expires"));
    }

    [Fact]
    public void ValidateForGermanRental_LicenseNotHeldLongEnough_ReturnsIssue()
    {
        // Arrange
        var sixMonthsAgo = Today.AddMonths(-6);
        var license = DriversLicense.Of("ABC12345", "Germany", sixMonthsAgo, FiveYearsFromNow);

        // Act
        var result = license.ValidateForGermanRental(Today);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Issues.ShouldContain(i => i.Contains("1 year"));
    }

    [Fact]
    public void ValidateForGermanRental_NonEuLicense_ReturnsWarning()
    {
        // Arrange
        var license = DriversLicense.Of("ABC12345", "USA", TwoYearsAgo, FiveYearsFromNow);

        // Act
        var result = license.ValidateForGermanRental(Today);

        // Assert - Non-EU license generates a warning but is still valid
        result.Issues.ShouldContain(i => i.Contains("International Driving Permit"));
    }

    [Fact]
    public void ValidateForGermanRental_MultipleIssues_ReturnsAllIssues()
    {
        // Arrange
        var sixMonthsAgo = Today.AddMonths(-6);
        var expiryDate = Today.AddDays(-10);
        var license = DriversLicense.Of("ABC12345", "USA", sixMonthsAgo, expiryDate);

        // Act
        var result = license.ValidateForGermanRental(Today);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Issues.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void LicenseValidationResult_Valid_HasNoIssues()
    {
        // Act
        var result = LicenseValidationResult.Valid;

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Issues.ShouldBeEmpty();
    }
}
