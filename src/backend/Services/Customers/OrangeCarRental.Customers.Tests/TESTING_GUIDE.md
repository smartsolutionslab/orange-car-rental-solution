# Testing Guide for CQRS Handlers

This guide provides patterns and best practices for testing command and query handlers in the Orange Car Rental system.

## Table of Contents

1. [Test Structure](#test-structure)
2. [Command Handler Testing Pattern](#command-handler-testing-pattern)
3. [Query Handler Testing Pattern](#query-handler-testing-pattern)
4. [Testing Checklist](#testing-checklist)
5. [Common Patterns](#common-patterns)
6. [Tools and Libraries](#tools-and-libraries)

---

## Test Structure

### Naming Convention

```csharp
{HandlerName}Tests.cs
```

Examples:
- `RegisterCustomerCommandHandlerTests.cs`
- `GetCustomerQueryHandlerTests.cs`
- `UpdateCustomerProfileCommandHandlerTests.cs`

### Test Method Naming

```csharp
{MethodName}_{Scenario}_{ExpectedBehavior}
```

Examples:
- `HandleAsync_WithValidCommand_CreatesCustomerSuccessfully`
- `HandleAsync_WithDuplicateEmail_ThrowsInvalidOperationException`
- `HandleAsync_WithNonExistentId_ReturnsNull`

### Test Organization with Regions

```csharp
#region Test Fixtures and Setup
// Constructor and mocks setup
#endregion

#region Happy Path Tests
// Primary success scenarios
#endregion

#region Error Cases
// Business rule violations
#endregion

#region Edge Cases
// Boundary conditions
#endregion

#region Helper Methods
// Test data builders
#endregion
```

---

## Command Handler Testing Pattern

### Required Test Categories

#### 1. Happy Path Tests
Test the primary successful execution path.

```csharp
[Fact]
public async Task HandleAsync_WithValidCommand_ExecutesSuccessfully()
{
    // Arrange - Setup test data and mocks
    var command = CreateValidCommand();
    SetupSuccessfulMocks();

    // Act - Execute handler
    var result = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert - Verify result and side effects
    result.Should().NotBeNull();
    VerifyRepositoryCalls();
}
```

#### 2. Error Cases - Business Rule Violations

```csharp
[Fact]
public async Task HandleAsync_WithInvalidInput_ThrowsException()
{
    // Arrange
    var command = CreateInvalidCommand();

    // Act
    var act = async () => await _handler.HandleAsync(command);

    // Assert
    await act.Should().ThrowAsync<InvalidOperationException>()
        .WithMessage("*expected message*");
}
```

#### 3. Validation Tests

```csharp
[Fact]
public async Task HandleAsync_WithEmptyId_ThrowsArgumentException()
{
    // Test that value object validation is enforced
    var act = () => CreateCommand(Guid.Empty);

    act.Should().Throw<ArgumentException>();
}
```

#### 4. Repository Interaction Tests

```csharp
[Fact]
public async Task HandleAsync_CallsRepositoryMethods_InCorrectOrder()
{
    // Arrange
    var command = CreateValidCommand();
    var callSequence = new List<string>();

    _mockRepository
        .Setup(r => r.AddAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()))
        .Callback(() => callSequence.Add("Add"))
        .Returns(Task.CompletedTask);

    _mockRepository
        .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .Callback(() => callSequence.Add("Save"))
        .Returns(Task.CompletedTask);

    // Act
    await _handler.HandleAsync(command);

    // Assert
    callSequence.Should().ContainInOrder("Add", "Save");
}
```

### Command Handler Test Checklist

- [ ] Happy path with valid input
- [ ] Business rule violations throw correct exceptions
- [ ] Value object validation enforced
- [ ] Repository.AddAsync called once
- [ ] Repository.SaveChangesAsync called once
- [ ] Aggregate state is correct
- [ ] Result DTO contains expected data
- [ ] Duplicate checks work correctly (if applicable)
- [ ] Cancellation token propagated
- [ ] No unintended side effects

---

## Query Handler Testing Pattern

### Required Test Categories

#### 1. Happy Path - Entity Found

```csharp
[Fact]
public async Task HandleAsync_WithExistingId_ReturnsDto()
{
    // Arrange
    var entity = CreateTestEntity();
    _mockRepository
        .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(entity);

    // Act
    var result = await _handler.HandleAsync(query);

    // Assert
    result.Should().NotBeNull();
    result.Property.Should().Be(entity.Property.Value);
}
```

#### 2. Not Found Case

```csharp
[Fact]
public async Task HandleAsync_WithNonExistentId_ReturnsNull()
{
    // Arrange
    _mockRepository
        .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
        .ReturnsAsync((Entity?)null);

    // Act
    var result = await _handler.HandleAsync(query);

    // Assert
    result.Should().BeNull();
}
```

#### 3. Data Mapping Tests

```csharp
[Fact]
public async Task HandleAsync_MapsAllFieldsCorrectly()
{
    // Verify all domain properties are mapped to DTO
    // Especially important for value objects
}
```

#### 4. Filtering and Pagination (for search queries)

```csharp
[Fact]
public async Task HandleAsync_WithFilters_AppliesCorrectly()
{
    // Test that query parameters are correctly passed to repository
}
```

### Query Handler Test Checklist

- [ ] Returns correct DTO when entity found
- [ ] Returns null when entity not found
- [ ] All domain fields mapped to DTO
- [ ] Value objects correctly converted
- [ ] Collections properly enumerated
- [ ] No mutations occur
- [ ] Repository called exactly once
- [ ] Filters applied correctly (search queries)
- [ ] Pagination works correctly (search queries)
- [ ] Performance within acceptable range

---

## Common Patterns

### 1. Mock Setup Pattern

```csharp
// Successful repository operation
_mockRepository
    .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
    .ReturnsAsync(entity);

// Not found
_mockRepository
    .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
    .ReturnsAsync((Entity?)null);

// Capture callback for verification
Entity? capturedEntity = null;
_mockRepository
    .Setup(r => r.AddAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()))
    .Callback<Entity, CancellationToken>((e, _) => capturedEntity = e)
    .Returns(Task.CompletedTask);
```

### 2. Test Data Builders

```csharp
private static MyCommand CreateValidCommand()
{
    return new MyCommand
    {
        Property1 = ValueObject.Of("valid value"),
        Property2 = ValueObject.Of("another value"),
        // ... all required properties
    };
}

// Variations
private static MyCommand CreateCommandWithInvalidEmail()
{
    return CreateValidCommand() with
    {
        Email = Email.Of("invalid")
    };
}
```

### 3. FluentAssertions Patterns

```csharp
// Object assertions
result.Should().NotBeNull();
result.Should().BeEquivalentTo(expected);

// String assertions
result.Message.Should().Contain("success");
result.Message.Should().MatchRegex(@"^\d+");

// Collection assertions
result.Items.Should().HaveCount(5);
result.Items.Should().ContainSingle(x => x.Id == targetId);

// Date/Time assertions
result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

// Exception assertions
act.Should().ThrowAsync<InvalidOperationException>()
    .WithMessage("*Customer*not found*");
```

### 4. Verify Pattern

```csharp
// Verify method called once
_mockRepository.Verify(
    r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
    Times.Once);

// Verify method never called
_mockRepository.Verify(
    r => r.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
    Times.Never);

// Verify with specific parameters
_mockRepository.Verify(
    r => r.GetByIdAsync(expectedId, It.IsAny<CancellationToken>()),
    Times.Once);
```

---

## Testing Checklist

### Before Writing Tests

- [ ] Understand the handler's business logic
- [ ] Identify all dependencies (repositories, services)
- [ ] List all success paths
- [ ] List all error conditions
- [ ] Identify edge cases

### For Each Test

- [ ] Descriptive test name
- [ ] AAA pattern (Arrange, Act, Assert)
- [ ] One logical assertion per test
- [ ] Clear comments for complex setups
- [ ] Proper use of mocks
- [ ] Verify side effects
- [ ] Clean up resources if needed

### After Writing Tests

- [ ] All tests pass
- [ ] Tests are independent (can run in any order)
- [ ] No test data pollution
- [ ] Tests run quickly (< 100ms each typically)
- [ ] Code coverage meets standards
- [ ] Tests are maintainable

---

## Tools and Libraries

### Test Framework
- **xUnit** - Primary test framework
- Convention: One test class per handler
- Use `[Fact]` for single test cases
- Use `[Theory]` with `[InlineData]` for parameterized tests

### Assertion Library
- **FluentAssertions** - Readable assertion syntax
- Provides detailed failure messages
- Better debugging experience

### Mocking Library
- **Moq** - Mocking framework
- Mock repositories and external services
- Never mock value objects or domain entities

### Example Theory Test

```csharp
[Theory]
[InlineData("john@example.com", true)]
[InlineData("jane@example.com", true)]
[InlineData("", false)]
[InlineData(null, false)]
public async Task HandleAsync_WithVariousEmails_ProducesExpectedResult(
    string email,
    bool shouldSucceed)
{
    // Parameterized test for multiple scenarios
}
```

---

## Example: Complete Test Class Structure

```csharp
using FluentAssertions;
using Moq;
using Xunit;

public sealed class MyCommandHandlerTests
{
    #region Test Fixtures and Setup

    private readonly Mock<IRepository> _mockRepository;
    private readonly MyCommandHandler _handler;

    public MyCommandHandlerTests()
    {
        _mockRepository = new Mock<IRepository>();
        _handler = new MyCommandHandler(_mockRepository.Object);
    }

    #endregion

    #region Happy Path Tests

    [Fact]
    public async Task HandleAsync_WithValidCommand_Succeeds()
    {
        // Arrange, Act, Assert
    }

    #endregion

    #region Error Cases

    [Fact]
    public async Task HandleAsync_WithInvalidInput_ThrowsException()
    {
        // Test error handling
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task HandleAsync_WithBoundaryCondition_HandlesCorrectly()
    {
        // Test edge cases
    }

    #endregion

    #region Helper Methods

    private static MyCommand CreateValidCommand()
    {
        return new MyCommand { /* ... */ };
    }

    #endregion
}
```

---

## Best Practices

1. **Test Behavior, Not Implementation**
   - Focus on what the handler does, not how it does it
   - Avoid testing internal implementation details

2. **Use Descriptive Names**
   - Test names should describe the scenario and expected outcome
   - Anyone should understand what's being tested

3. **Keep Tests Independent**
   - No shared state between tests
   - Each test should setup its own data

4. **Mock Only External Dependencies**
   - Mock repositories and external services
   - Don't mock value objects or domain entities
   - Don't mock the system under test

5. **Assert All Side Effects**
   - Verify return values
   - Verify repository calls
   - Verify state changes

6. **Test One Thing at a Time**
   - Each test should have a single logical assertion
   - Use multiple tests for multiple scenarios

7. **Make Tests Readable**
   - Use AAA pattern
   - Add comments for complex setups
   - Use meaningful variable names

8. **Keep Tests Fast**
   - Unit tests should run in milliseconds
   - No database or file system access
   - Use mocks for all I/O

---

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests in specific project
dotnet test Services/Customers/OrangeCarRental.Customers.Tests

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "FullyQualifiedName~RegisterCustomerCommandHandlerTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~RegisterCustomerCommandHandlerTests.HandleAsync_WithValidCommand_CreatesCustomerSuccessfully"
```

---

## Next Steps

1. Review the example test files:
   - `RegisterCustomerCommandHandlerTests.cs`
   - `GetCustomerQueryHandlerTests.cs`

2. Use these as templates for creating tests for:
   - `ChangeCustomerStatusCommandHandler`
   - `UpdateCustomerProfileCommandHandler`
   - `UpdateDriversLicenseCommandHandler`
   - `GetCustomerByEmailQueryHandler`
   - `SearchCustomersQueryHandler`

3. Aim for minimum 80% code coverage

4. Write tests BEFORE fixing bugs (TDD for bug fixes)

5. Update tests when handlers change

---

## Questions or Issues?

If you encounter issues or have questions about testing patterns:

1. Review existing test files in Reservations and Fleet services
2. Check this guide for common patterns
3. Ensure all mocks are properly configured
4. Verify test isolation (no shared state)

Happy Testing! 🧪
