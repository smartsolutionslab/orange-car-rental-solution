using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Sepa;

public class MandateReferenceTests
{
    [Fact]
    public void Create_ValidSequenceNumber_CreatesMandateReference()
    {
        // Arrange
        var sequenceNumber = 1;
        var date = new DateOnly(2025, 1, 15);

        // Act
        var reference = MandateReference.Create(sequenceNumber, date);

        // Assert
        reference.Value.ShouldBe("OCR-SEPA-20250115-000001");
    }

    [Fact]
    public void Create_HighSequenceNumber_FormatsCorrectly()
    {
        // Arrange
        var sequenceNumber = 123456;
        var date = new DateOnly(2025, 12, 31);

        // Act
        var reference = MandateReference.Create(sequenceNumber, date);

        // Assert
        reference.Value.ShouldBe("OCR-SEPA-20251231-123456");
    }

    [Fact]
    public void Create_WithoutDate_UsesCurrentDate()
    {
        // Arrange
        var sequenceNumber = 1;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var reference = MandateReference.Create(sequenceNumber);

        // Assert
        reference.Value.ShouldStartWith($"OCR-SEPA-{today:yyyyMMdd}-");
    }

    [Fact]
    public void Create_ZeroSequenceNumber_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => MandateReference.Create(0));
    }

    [Fact]
    public void Create_NegativeSequenceNumber_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => MandateReference.Create(-1));
    }

    [Fact]
    public void Parse_ValidMandateReference_ParsesCorrectly()
    {
        // Arrange
        var value = "OCR-SEPA-20250115-000123";

        // Act
        var reference = MandateReference.Parse(value);

        // Assert
        reference.Value.ShouldBe(value);
    }

    [Fact]
    public void Parse_ValidLowercaseReference_NormalizesToUppercase()
    {
        // Arrange
        var value = "ocr-sepa-20250115-000123";

        // Act
        var reference = MandateReference.Parse(value);

        // Assert
        reference.Value.ShouldBe("OCR-SEPA-20250115-000123");
    }

    [Fact]
    public void Parse_ValidReferenceWithWhitespace_TrimsWhitespace()
    {
        // Arrange
        var value = "  OCR-SEPA-20250115-000123  ";

        // Act
        var reference = MandateReference.Parse(value);

        // Assert
        reference.Value.ShouldBe("OCR-SEPA-20250115-000123");
    }

    [Fact]
    public void Parse_InvalidPrefix_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => MandateReference.Parse("XXX-SEPA-20250115-000001"));
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => MandateReference.Parse("OCR-SEPA-2025"));
    }

    [Fact]
    public void Parse_InvalidDate_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => MandateReference.Parse("OCR-SEPA-XXXXXXXX-000001"));
    }

    [Fact]
    public void Parse_NullOrWhitespace_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => MandateReference.Parse(null!));
        Should.Throw<ArgumentException>(() => MandateReference.Parse(""));
        Should.Throw<ArgumentException>(() => MandateReference.Parse("   "));
    }

    [Fact]
    public void TryParse_ValidReference_ReturnsTrue()
    {
        // Arrange
        var value = "OCR-SEPA-20250115-000123";

        // Act
        var result = MandateReference.TryParse(value, out var reference);

        // Assert
        result.ShouldBeTrue();
        reference.ShouldNotBeNull();
        reference!.Value.ShouldBe(value);
    }

    [Fact]
    public void TryParse_InvalidReference_ReturnsFalse()
    {
        // Act
        var result = MandateReference.TryParse("invalid", out var reference);

        // Assert
        result.ShouldBeFalse();
        reference.ShouldBeNull();
    }

    [Fact]
    public void TryParse_NullReference_ReturnsFalse()
    {
        // Act
        var result = MandateReference.TryParse(null, out var reference);

        // Assert
        result.ShouldBeFalse();
        reference.ShouldBeNull();
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var reference = MandateReference.Create(42, new DateOnly(2025, 6, 15));

        // Act
        var result = reference.ToString();

        // Assert
        result.ShouldBe("OCR-SEPA-20250615-000042");
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        // Arrange
        var reference1 = MandateReference.Parse("OCR-SEPA-20250115-000001");
        var reference2 = MandateReference.Parse("OCR-SEPA-20250115-000001");

        // Assert
        reference1.ShouldBe(reference2);
    }

    [Fact]
    public void Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var reference1 = MandateReference.Parse("OCR-SEPA-20250115-000001");
        var reference2 = MandateReference.Parse("OCR-SEPA-20250115-000002");

        // Assert
        reference1.ShouldNotBe(reference2);
    }
}
