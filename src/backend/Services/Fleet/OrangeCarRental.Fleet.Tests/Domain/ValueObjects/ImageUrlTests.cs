using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class ImageUrlTests
{
    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("http://cdn.example.com/vehicles/bmw-x5.png")]
    [InlineData("https://images.example.com/path/to/image.webp")]
    public void From_WithValidUrl_ShouldCreate(string validUrl)
    {
        // Act
        var imageUrl = ImageUrl.From(validUrl);

        // Assert
        imageUrl.Value.ShouldBe(validUrl);
    }

    [Fact]
    public void From_WithLeadingAndTrailingWhitespace_ShouldTrim()
    {
        // Act
        var imageUrl = ImageUrl.From("  https://example.com/image.jpg  ");

        // Assert
        imageUrl.Value.ShouldBe("https://example.com/image.jpg");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidUrl)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => ImageUrl.From(invalidUrl));
    }

    [Fact]
    public void From_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => ImageUrl.From(null!));
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("example.com/image.jpg")]
    [InlineData("/path/to/image.jpg")]
    public void From_WithInvalidUrlFormat_ShouldThrowArgumentException(string invalidUrl)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => ImageUrl.From(invalidUrl));
    }

    [Theory]
    [InlineData("ftp://example.com/image.jpg")]
    [InlineData("file:///C:/images/test.jpg")]
    public void From_WithInvalidScheme_ShouldThrowArgumentException(string invalidUrl)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => ImageUrl.From(invalidUrl));
    }

    [Fact]
    public void From_WithTooLongUrl_ShouldThrowArgumentException()
    {
        // Arrange - URL exceeding 500 characters
        var longUrl = "https://example.com/" + new string('a', 490);

        // Act & Assert
        Should.Throw<ArgumentException>(() => ImageUrl.From(longUrl));
    }

    [Theory]
    [InlineData("https://example.com/image.jpg", true)]
    [InlineData("http://example.com/image.png", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("not-a-url", false)]
    [InlineData("ftp://example.com/image.jpg", false)]
    public void TryParse_ShouldReturnExpectedResult(string? input, bool expectedResult)
    {
        // Act
        var result = ImageUrl.TryParse(input, out var imageUrl);

        // Assert
        result.ShouldBe(expectedResult);
        if (expectedResult)
        {
            imageUrl.Value.ShouldNotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void TryParse_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = ImageUrl.TryParse(null, out _);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("https://example.com/image.jpg", true)]
    [InlineData("https://example.com/image.jpeg", true)]
    [InlineData("https://example.com/image.png", true)]
    [InlineData("https://example.com/image.gif", true)]
    [InlineData("https://example.com/image.webp", true)]
    [InlineData("https://example.com/image.svg", true)]
    [InlineData("https://example.com/image", false)]
    [InlineData("https://example.com/image.pdf", false)]
    [InlineData("https://example.com/image.txt", false)]
    public void HasImageExtension_ShouldReturnExpectedResult(string url, bool expected)
    {
        // Arrange
        var imageUrl = ImageUrl.From(url);

        // Act
        var result = imageUrl.HasImageExtension();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var imageUrl = ImageUrl.From("https://example.com/image.jpg");

        // Act
        string urlString = imageUrl;

        // Assert
        urlString.ShouldBe("https://example.com/image.jpg");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var imageUrl = ImageUrl.From("https://example.com/image.jpg");

        // Act
        var result = imageUrl.ToString();

        // Assert
        result.ShouldBe("https://example.com/image.jpg");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var url1 = ImageUrl.From("https://example.com/image.jpg");
        var url2 = ImageUrl.From("https://example.com/image.jpg");

        // Act & Assert
        url1.ShouldBe(url2);
        (url1 == url2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var url1 = ImageUrl.From("https://example.com/image1.jpg");
        var url2 = ImageUrl.From("https://example.com/image2.jpg");

        // Act & Assert
        url1.ShouldNotBe(url2);
        (url1 != url2).ShouldBeTrue();
    }
}
