# Fluent Validation Guide - Ensure Pattern

## Overview

The `Ensure` fluent validation library provides a clean, readable way to validate parameters and values throughout the codebase.

## Basic Usage

```csharp
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

// String validation
Ensure.That(value, nameof(value))
    .IsNotNullOrWhiteSpace()
    .AndHasMinLength(4)
    .AndHasMaxLength(40);

// Numeric validation
Ensure.That(age, nameof(age))
    .IsGreaterThan(0)
    .AndIsLessThan(150);

// Collection validation
Ensure.That(items, nameof(items))
    .IsNotNullOrEmpty()
    .AndHasMinCount(1)
    .AndContainsNoNulls();
```

## Available Extension Methods

### String Validation (`EnsureStringExtensions`)

- `IsNotNull()` - Ensures string is not null
- `IsNotNullOrEmpty()` - Ensures string is not null or empty
- `IsNotNullOrWhiteSpace()` - Ensures string is not null, empty, or whitespace
- `AndHasMinLength(int)` - Ensures minimum length
- `AndHasMaxLength(int)` - Ensures maximum length
- `AndHasLengthBetween(int, int)` - Ensures length is within range
- `AndIsLongerThan(int)` - Ensures length is strictly greater than
- `AndIsShorterThan(int)` - Ensures length is strictly less than
- `AndMatches(string pattern)` - Ensures matches regex pattern
- `AndDoesNotMatch(string pattern)` - Ensures does not match regex pattern
- `AndContains(string)` - Ensures contains substring
- `AndDoesNotContain(string)` - Ensures does not contain substring
- `AndStartsWith(string)` - Ensures starts with prefix
- `AndEndsWith(string)` - Ensures ends with suffix
- `AndEquals(string)` - Ensures equals specified value
- `AndIsValidEmail()` - Ensures valid email format
- `AndIsValidUrl()` - Ensures valid URL format

### Numeric Validation (`EnsureNumericExtensions`)

- `IsGreaterThan<T>(T)` - Ensures value is greater than minimum
- `IsGreaterThanOrEqual<T>(T)` - Ensures value is >= minimum
- `IsLessThan<T>(T)` - Ensures value is less than maximum
- `IsLessThanOrEqual<T>(T)` - Ensures value is <= maximum
- `AndIsBetween<T>(T, T)` - Ensures value is within range (inclusive)
- `IsPositive()` - Ensures value is greater than zero
- `IsNegative()` - Ensures value is less than zero
- `IsZero()` - Ensures value equals zero
- `IsNotZero()` - Ensures value does not equal zero
- `And()` - Fluent continuation word for readability
- `AndIsLessThan<T>(T)` - Alias for IsLessThan
- `AndIsGreaterThan<T>(T)` - Alias for IsGreaterThan

### Object Validation (`EnsureObjectExtensions`)

- `IsNotNull()` - Ensures object is not null (works with classes and nullable structs)
- `IsNull()` - Ensures object is null
- `AndEquals<T>(T)` - Ensures value equals expected
- `AndNotEquals<T>(T)` - Ensures value does not equal specified value
- `AndIsOfType<T, TExpected>()` - Ensures value is of specified type
- `AndSatisfies<T>(Func<T, bool>)` - Ensures custom predicate is satisfied

### Collection Validation (`EnsureCollectionExtensions`)

- `IsNotNull()` - Ensures collection is not null
- `IsNotEmpty()` - Ensures collection is not empty
- `IsNotNullOrEmpty()` - Ensures collection is not null or empty
- `AndHasMinCount(int)` - Ensures minimum item count
- `AndHasMaxCount(int)` - Ensures maximum item count
- `AndHasCountBetween(int, int)` - Ensures count is within range
- `AndHasExactCount(int)` - Ensures exact item count
- `AndContains<T>(T)` - Ensures collection contains item
- `AndDoesNotContain<T>(T)` - Ensures collection does not contain item
- `AndAllItemsSatisfy<T>(Func<T, bool>)` - Ensures all items satisfy predicate
- `AndAnyItemSatisfies<T>(Func<T, bool>)` - Ensures at least one item satisfies predicate
- `AndContainsNoNulls()` - Ensures no null items
- `AndContainsDistinctItems()` - Ensures no duplicate items

## Real-World Examples

### Value Object Validation

```csharp
public readonly record struct EmailAddress
{
    private readonly string value;

    private EmailAddress(string value)
    {
        this.value = value;
    }

    public static EmailAddress From(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(256)
            .AndIsValidEmail();

        return new EmailAddress(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => value;
}
```

### Domain Entity Validation

```csharp
public sealed class Customer : AggregateRoot<CustomerId>
{
    public static Customer Register(
        string firstName,
        string lastName,
        Email email,
        DateOnly dateOfBirth)
    {
        // Validate parameters
        Ensure.That(firstName, nameof(firstName))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(50);

        Ensure.That(lastName, nameof(lastName))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(50);

        Ensure.That(email, nameof(email))
            .IsNotNull();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;

        Ensure.That(age, nameof(dateOfBirth))
            .IsGreaterThanOrEqual(18)
            .AndIsLessThan(120);

        return new Customer(/* ... */);
    }
}
```

### Command Handler Validation

```csharp
public sealed class CreateReservationCommandHandler
{
    public async Task<CreateReservationResult> HandleAsync(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate command
        Ensure.That(command, nameof(command))
            .IsNotNull();

        Ensure.That(command.VehicleId, nameof(command.VehicleId))
            .IsNotNull()
            .AndNotEquals(Guid.Empty);

        Ensure.That(command.RentalDays, nameof(command.RentalDays))
            .IsGreaterThan(0)
            .AndIsLessThan(365);

        // Process command...
    }
}
```

### Complex Custom Validation

```csharp
public static LocationCode From(string value)
{
    Ensure.That(value, nameof(value))
        .IsNotNullOrWhiteSpace()
        .AndHasLengthBetween(3, 10)
        .AndMatches(@"^[A-Z]{3,5}-\d{2}$", "format 'XXX-NN' or 'XXXXX-NN'")
        .AndSatisfies(
            code => !code.StartsWith("TST"),
            "Location code cannot start with 'TST' (reserved for test data)");

    return new LocationCode(value.ToUpperInvariant());
}
```

## Migration Strategy

### Before (Old Style)

```csharp
public static EmailAddress From(string value)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

    if (!IsValidEmail(value))
    {
        throw new ArgumentException($"Invalid email address format: {value}", nameof(value));
    }

    return new EmailAddress(value.Trim().ToLowerInvariant());
}
```

### After (Ensure Pattern)

```csharp
public static EmailAddress From(string value)
{
    Ensure.That(value, nameof(value))
        .IsNotNullOrWhiteSpace()
        .AndIsValidEmail();

    return new EmailAddress(value.Trim().ToLowerInvariant());
}
```

## Benefits

1. **Readability**: Validation logic reads like natural language
2. **Consistency**: Same validation style across entire codebase
3. **Extensibility**: Easy to add new validation methods via extensions
4. **Error Messages**: Automatically generates clear, descriptive error messages
5. **Chainable**: Multiple validations in a single fluent chain
6. **Type Safety**: Strongly typed with IntelliSense support

## When to Use

Use `Ensure.That()` for:
- Parameter validation in factory methods
- Guard clauses in constructors
- Command/query validation
- Value object creation
- Domain entity invariant checks
- Service method input validation

## Notes

- All validation methods throw `ArgumentException` or `ArgumentNullException` on failure
- Error messages automatically include the parameter name and expected/actual values
- Methods starting with "Is" are typically the first in a chain
- Methods starting with "And" continue an existing chain
- The library is located in `OrangeCarRental.BuildingBlocks.Domain.Validation` namespace
