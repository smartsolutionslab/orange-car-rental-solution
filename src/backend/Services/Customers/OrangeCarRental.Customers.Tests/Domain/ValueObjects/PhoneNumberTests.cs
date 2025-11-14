using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+491512345678", "+491512345678")]
    [InlineData("+49 151 12345678", "+491512345678")]
    [InlineData("+49-151-12345678", "+491512345678")]
    [InlineData("+49 (151) 1234-5678", "+491512345678")]
    [InlineData("+49/151/12345678", "+491512345678")]
    public void Of_WithValidInternationalFormat_ShouldNormalize(string input, string expected)
    {
        // Act
        var phoneNumber = PhoneNumber.Of(input);

        // Assert
        phoneNumber.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("01512345678", "+491512345678")]
    [InlineData("0151 12345678", "+491512345678")]
    [InlineData("0151-1234-5678", "+491512345678")]
    [InlineData("  0151 12345678  ", "+491512345678")]
    public void Of_WithGermanDomesticFormat_ShouldConvertToInternational(string input, string expected)
    {
        // Act
        var phoneNumber = PhoneNumber.Of(input);

        // Assert
        phoneNumber.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("00491512345678", "+491512345678")]
    [InlineData("0049 151 12345678", "+491512345678")]
    public void Of_WithDoubleZeroPrefix_ShouldConvertToPlus(string input, string expected)
    {
        // Act
        var phoneNumber = PhoneNumber.Of(input);

        // Assert
        phoneNumber.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Of_WithNullOrWhitespace_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => PhoneNumber.Of(invalidPhone));
    }

    [Theory]
    [InlineData("+441234567890")] // UK number
    [InlineData("+11234567890")] // US number
    [InlineData("+33123456789")] // French number
    [InlineData("1234567890")] // No country code
    public void Of_WithNonGermanNumber_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => PhoneNumber.Of(invalidPhone));
    }

    [Fact]
    public void Of_WithTooShortNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var shortNumber = "+4915"; // Less than minimum length

        // Act & Assert
        Should.Throw<ArgumentException>(() => PhoneNumber.Of(shortNumber));
    }

    [Fact]
    public void Of_WithTooLongNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var longNumber = "+4912345678901234567"; // More than 16 characters

        // Act & Assert
        Should.Throw<ArgumentException>(() => PhoneNumber.Of(longNumber));
    }

    [Theory]
    [InlineData("+49012345678")] // First digit after country code is 0
    [InlineData("+490151234567")] // Same issue
    public void Of_WithZeroAfterCountryCode_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => PhoneNumber.Of(invalidPhone));
    }

    [Theory]
    [InlineData("+49abc123456")] // Contains letters
    [InlineData("+4915 abc 1234")] // Contains letters in middle
    public void Of_WithNonDigits_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => PhoneNumber.Of(invalidPhone));
    }

    [Theory]
    [InlineData("+491512345678", "+49 151 2345678")]
    [InlineData("+4930123456", "+49 301 23456")]
    [InlineData("+4989123456789", "+49 891 23456789")]
    public void FormattedValue_ShouldFormatForDisplay(string input, string expectedFormat)
    {
        // Act
        var phoneNumber = PhoneNumber.Of(input);

        // Assert
        phoneNumber.FormattedValue.ShouldBe(expectedFormat);
    }

    [Fact]
    public void Anonymized_ShouldCreateAnonymizedPhoneNumber()
    {
        // Act
        var phoneNumber = PhoneNumber.Anonymized();

        // Assert
        phoneNumber.Value.ShouldBe("+490000000000");
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var phoneNumber = PhoneNumber.Of("+491512345678");

        // Act
        string phoneString = phoneNumber;

        // Assert
        phoneString.ShouldBe("+491512345678");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedValue()
    {
        // Arrange
        var phoneNumber = PhoneNumber.Of("+491512345678");

        // Act
        var result = phoneNumber.ToString();

        // Assert
        result.ShouldBe("+49 151 2345678");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var phone1 = PhoneNumber.Of("0151 12345678");
        var phone2 = PhoneNumber.Of("+49 151 12345678");

        // Act & Assert
        phone1.ShouldBe(phone2);
        (phone1 == phone2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var phone1 = PhoneNumber.Of("+491512345678");
        var phone2 = PhoneNumber.Of("+491612345678");

        // Act & Assert
        phone1.ShouldNotBe(phone2);
        (phone1 != phone2).ShouldBeTrue();
    }

    [Theory]
    [InlineData("+49 30 12345678")] // Berlin
    [InlineData("+49 89 12345678")] // Munich
    [InlineData("+49 40 12345678")] // Hamburg
    [InlineData("+49 221 1234567")] // Cologne
    [InlineData("+49 151 12345678")] // Mobile (Vodafone)
    [InlineData("+49 170 12345678")] // Mobile (T-Mobile)
    [InlineData("+49 176 12345678")] // Mobile (O2)
    public void Of_WithRealGermanNumbers_ShouldSucceed(string germanPhone)
    {
        // Act
        var phoneNumber = PhoneNumber.Of(germanPhone);

        // Assert
        phoneNumber.Value.ShouldStartWith("+49");
    }
}
