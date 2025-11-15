using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class SeatingCapacityTests
{
    [Theory]
    [InlineData(2)] // Minimum (sports cars, small vehicles)
    [InlineData(4)] // Sedan
    [InlineData(5)] // Standard sedan
    [InlineData(7)] // SUV/Minivan
    [InlineData(9)] // Maximum (transporter/van)
    public void Of_WithValidCapacity_ShouldCreateSeatingCapacity(int validCapacity)
    {
        // Act
        var capacity = SeatingCapacity.Of(validCapacity);

        // Assert
        capacity.Value.ShouldBe(validCapacity);
    }

    [Fact]
    public void Of_WithCapacityLessThan2_ShouldThrowArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => SeatingCapacity.Of(1));
        ex.Message.ShouldContain("at least 2");
    }

    [Fact]
    public void Of_WithCapacityGreaterThan9_ShouldThrowArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => SeatingCapacity.Of(10));
        ex.Message.ShouldContain("cannot exceed 9");
    }

    [Fact]
    public void Of_WithExactly2Seats_ShouldSucceed()
    {
        // Act
        var capacity = SeatingCapacity.Of(2);

        // Assert
        capacity.Value.ShouldBe(2);
    }

    [Fact]
    public void Of_WithExactly9Seats_ShouldSucceed()
    {
        // Act
        var capacity = SeatingCapacity.Of(9);

        // Assert
        capacity.Value.ShouldBe(9);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToInt()
    {
        // Arrange
        var capacity = SeatingCapacity.Of(5);

        // Act
        int capacityInt = capacity;

        // Assert
        capacityInt.ShouldBe(5);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var capacity = SeatingCapacity.Of(5);

        // Act
        var result = capacity.ToString();

        // Assert
        result.ShouldBe("5");
    }

    [Fact]
    public void ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var small = SeatingCapacity.Of(2);
        var medium = SeatingCapacity.Of(5);
        var mediumCopy = SeatingCapacity.Of(5);
        var large = SeatingCapacity.Of(9);

        // Act & Assert
        (small < medium).ShouldBeTrue();
        (small <= medium).ShouldBeTrue();
        (large > medium).ShouldBeTrue();
        (large >= medium).ShouldBeTrue();
        (medium <= mediumCopy).ShouldBeTrue();
        (medium >= mediumCopy).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var capacity1 = SeatingCapacity.Of(5);
        var capacity2 = SeatingCapacity.Of(5);

        // Act & Assert
        capacity1.ShouldBe(capacity2);
        (capacity1 == capacity2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var capacity1 = SeatingCapacity.Of(5);
        var capacity2 = SeatingCapacity.Of(7);

        // Act & Assert
        capacity1.ShouldNotBe(capacity2);
        (capacity1 != capacity2).ShouldBeTrue();
    }
}
