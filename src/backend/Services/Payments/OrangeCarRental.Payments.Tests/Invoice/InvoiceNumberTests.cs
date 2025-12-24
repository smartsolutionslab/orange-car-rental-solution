using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Invoice;

public class InvoiceNumberTests
{
    [Fact]
    public void Create_ValidSequenceNumber_CreatesInvoiceNumber()
    {
        // Arrange
        var sequenceNumber = 1;
        var year = 2025;

        // Act
        var invoiceNumber = InvoiceNumber.Create(sequenceNumber, year);

        // Assert
        invoiceNumber.Value.ShouldBe("OCR-2025-000001");
        invoiceNumber.Year.ShouldBe(2025);
        invoiceNumber.SequenceNumber.ShouldBe(1);
    }

    [Fact]
    public void Create_HighSequenceNumber_FormatsCorrectly()
    {
        // Arrange
        var sequenceNumber = 123456;
        var year = 2025;

        // Act
        var invoiceNumber = InvoiceNumber.Create(sequenceNumber, year);

        // Assert
        invoiceNumber.Value.ShouldBe("OCR-2025-123456");
    }

    [Fact]
    public void Create_WithoutYear_UsesCurrentYear()
    {
        // Arrange
        var sequenceNumber = 1;
        var currentYear = DateTime.UtcNow.Year;

        // Act
        var invoiceNumber = InvoiceNumber.Create(sequenceNumber);

        // Assert
        invoiceNumber.Year.ShouldBe(currentYear);
        invoiceNumber.Value.ShouldStartWith($"OCR-{currentYear}-");
    }

    [Fact]
    public void Create_ZeroSequenceNumber_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Create(0, 2025));
    }

    [Fact]
    public void Create_NegativeSequenceNumber_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Create(-1, 2025));
    }

    [Fact]
    public void Create_YearBefore2020_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Create(1, 2019));
    }

    [Fact]
    public void Create_YearAfter2099_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Create(1, 2100));
    }

    [Fact]
    public void Parse_ValidInvoiceNumber_ParsesCorrectly()
    {
        // Arrange
        var value = "OCR-2025-000123";

        // Act
        var invoiceNumber = InvoiceNumber.Parse(value);

        // Assert
        invoiceNumber.Value.ShouldBe("OCR-2025-000123");
        invoiceNumber.Year.ShouldBe(2025);
        invoiceNumber.SequenceNumber.ShouldBe(123);
    }

    [Fact]
    public void Parse_InvalidPrefix_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Parse("INV-2025-000001"));
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Parse("OCR-2025"));
    }

    [Fact]
    public void Parse_InvalidYear_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => InvoiceNumber.Parse("OCR-XXXX-000001"));
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var invoiceNumber = InvoiceNumber.Create(42, 2025);

        // Act
        var result = invoiceNumber.ToString();

        // Assert
        result.ShouldBe("OCR-2025-000042");
    }
}
