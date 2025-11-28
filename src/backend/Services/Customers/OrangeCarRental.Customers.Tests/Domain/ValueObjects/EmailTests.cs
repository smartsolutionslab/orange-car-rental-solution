using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("firstname.lastname@company.com")]
    [InlineData("email@subdomain.example.com")]
    public void Of_WithValidEmail_ShouldCreateEmail(string validEmail)
    {
        // Act
        var email = Email.From(validEmail);

        // Assert
        email.Value.ShouldBe(validEmail.ToLowerInvariant());
    }

    [Theory]
    [InlineData("  test@example.com  ", "test@example.com")]
    [InlineData("Test@Example.COM", "test@example.com")]
    [InlineData("  USER@DOMAIN.COM  ", "user@domain.com")]
    public void Of_ShouldNormalizeEmail_ToLowercaseAndTrim(string input, string expected)
    {
        // Act
        var email = Email.From(input);

        // Assert
        email.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Of_WithNullOrWhitespace_ShouldThrowArgumentException(string? invalidEmail)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => Email.From(invalidEmail!));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user name@example.com")]
    public void Of_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => Email.From(invalidEmail));
    }

    [Fact]
    public void Of_WithEmailExceeding254Characters_ShouldThrowArgumentException()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@test.com"; // > 254 characters

        // Act & Assert
        Should.Throw<ArgumentException>(() => Email.From(longEmail));
    }

    [Fact]
    public void Anonymized_ShouldCreateAnonymizedEmail()
    {
        // Act
        var email = Email.Anonymized();

        // Assert
        email.Value.ShouldStartWith("anonymized-");
        email.Value.ShouldEndWith("@gdpr-deleted.local");
    }

    [Fact]
    public void Anonymized_ShouldCreateUniqueEmails()
    {
        // Act
        var email1 = Email.Anonymized();
        var email2 = Email.Anonymized();

        // Assert
        email1.Value.ShouldNotBe(email2.Value);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var email = Email.From("test@example.com");

        // Act
        string emailString = email;

        // Assert
        emailString.ShouldBe("test@example.com");
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = Email.From("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        result.ShouldBe("test@example.com");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.From("test@example.com");
        var email2 = Email.From("test@example.com");

        // Act & Assert
        email1.ShouldBe(email2);
        (email1 == email2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var email1 = Email.From("test1@example.com");
        var email2 = Email.From("test2@example.com");

        // Act & Assert
        email1.ShouldNotBe(email2);
        (email1 != email2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_IsCaseInsensitive()
    {
        // Arrange
        var email1 = Email.From("Test@Example.COM");
        var email2 = Email.From("test@example.com");

        // Act & Assert
        email1.ShouldBe(email2);
    }
}
