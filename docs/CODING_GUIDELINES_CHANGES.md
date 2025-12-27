# Coding Guidelines Changes

## Date: 2025-11-05

## Overview
Updated project coding standards to improve code readability and implement modern validation patterns.

## Changes Made

### 1. .editorconfig Updates

#### Allow `var` Keyword Everywhere
**Before:**
```
csharp_style_var_for_built_in_types = false:warning
csharp_style_var_elsewhere = false:warning
```

**After:**
```
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_elsewhere = true:suggestion
```

**Impact:** Developers can now use `var` for all variable declarations when type is apparent.

```csharp
// Now allowed:
var customer = Customer.Register(...);
var count = 42;
var name = "John Doe";
```

#### Remove Underscore Requirement for Private Fields
**Before:**
```
dotnet_naming_rule.private_fields_should_be_prefixed_with_underscore.severity = warning
dotnet_naming_rule.private_fields_should_be_prefixed_with_underscore.style = begins_with_underscore
```

**After:**
```
dotnet_naming_rule.private_fields_should_be_camel_case.severity = warning
dotnet_naming_rule.private_fields_should_be_camel_case.style = camel_case
```

**Impact:** Private fields no longer require underscore prefix.

```csharp
// Before:
private readonly string _value;
private readonly int _count;

// After (both styles are now valid, choose one per project):
private readonly string value;
private readonly int count;
```

#### Allow Single-Line If Without Braces
**Before:**
```
csharp_prefer_braces = true:warning
```

**After:**
```
csharp_prefer_braces = when_multiline:suggestion
```

**Impact:** Single-line if statements no longer require braces.

```csharp
// Now allowed:
if (value is null) return;
if (count == 0) throw new InvalidOperationException();

// Multi-line still requires braces:
if (condition)
{
    DoSomething();
    DoSomethingElse();
}
```

### 2. New Fluent Validation Library

Created comprehensive validation framework using the **Ensure pattern**.

#### Files Created:
1. `BuildingBlocks/Validation/Ensure.cs` - Core fluent validation entry point
2. `BuildingBlocks/Validation/EnsureStringExtensions.cs` - String validation methods
3. `BuildingBlocks/Validation/EnsureObjectExtensions.cs` - Object validation methods
4. `BuildingBlocks/Validation/EnsureNumericExtensions.cs` - Numeric validation methods
5. `BuildingBlocks/Validation/EnsureCollectionExtensions.cs` - Collection validation methods

#### Usage Example:

```csharp
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

// String validation
Ensure.That(email, nameof(email))
    .IsNotNullOrWhiteSpace()
    .AndHasMaxLength(256)
    .AndIsValidEmail();

// Numeric validation
Ensure.That(age, nameof(age))
    .IsGreaterThan(0)
    .AndIsLessThan(150);

// Collection validation
Ensure.That(items, nameof(items))
    .IsNotNullOrEmpty()
    .AndHasMinCount(1)
    .AndContainsNoNulls();

// Custom conditions
Ensure.That(locationCode, nameof(locationCode))
    .IsNotNullOrWhiteSpace()
    .AndSatisfies(
        code => !code.StartsWith("TST"),
        "Location code cannot start with 'TST'");
```

### 3. Refactored Existing Code

#### EmailAddress Value Object
**Before:**
```csharp
public static EmailAddress Of(string value)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

    if (!IsValidEmail(value))
        throw new ArgumentException($"Invalid email address format: {value}", nameof(value));

    return new EmailAddress(value.Trim().ToLowerInvariant());
}

private static bool IsValidEmail(string email)
{
    return EmailRegex.IsMatch(email);
}
```

**After:**
```csharp
public static EmailAddress Of(string value)
{
    Ensure.That(value, nameof(value))
        .IsNotNullOrWhiteSpace()
        .AndHasMaxLength(256)
        .AndIsValidEmail();

    return new EmailAddress(value.Trim().ToLowerInvariant());
}
// IsValidEmail() method no longer needed - built into Ensure
```

### 4. Documentation Created

- `docs/VALIDATION_GUIDE.md` - Comprehensive guide to the Ensure validation pattern
- `docs/CODING_GUIDELINES_CHANGES.md` - This document

## Migration Path

### Phase 1: Use in New Code (Immediate)
- All new value objects, entities, and commands should use `Ensure.That()` for validation
- Use `var` where type is apparent
- Single-line if statements don't need braces

### Phase 2: Gradual Refactoring (Optional)
- Refactor existing validation code to use `Ensure.That()` when touching those files
- No need to refactor working code immediately
- Focus on files being actively modified

### Phase 3: Field Naming Cleanup (Optional)
- Decide on one standard: either camelCase or _camelCase for private fields
- Update all private fields to match chosen standard
- This can be done file-by-file or in a dedicated refactoring sprint

## Benefits

### Readability
```csharp
// Old style:
ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
if (value.Length < 4)
    throw new ArgumentException($"Value must be at least 4 characters", nameof(value));
if (value.Length > 40)
    throw new ArgumentException($"Value must be at most 40 characters", nameof(value));

// New style - reads like English:
Ensure.That(value, nameof(value))
    .IsNotNullOrWhiteSpace()
    .AndIsLongerThan(4)
    .AndHasMaxLength(40);
```

### Consistency
- Same validation pattern across entire codebase
- Automatic, consistent error messages
- Centralized validation logic

### Extensibility
- Easy to add new validation methods via extension methods
- Custom validations with `.AndSatisfies(predicate, message)`
- Domain-specific validators can be added per service

### Type Safety
- Full IntelliSense support
- Compile-time type checking
- Generic constraints ensure correct usage

## Validation Method Reference

### String Methods
- `IsNotNull()`, `IsNotNullOrEmpty()`, `IsNotNullOrWhiteSpace()`
- `AndHasMinLength(int)`, `AndHasMaxLength(int)`, `AndHasLengthBetween(int, int)`
- `AndIsLongerThan(int)`, `AndIsShorterThan(int)`
- `AndMatches(pattern)`, `AndDoesNotMatch(pattern)`
- `AndContains(string)`, `AndDoesNotContain(string)`
- `AndStartsWith(string)`, `AndEndsWith(string)`
- `AndIsValidEmail()`, `AndIsValidUrl()`

### Numeric Methods
- `IsGreaterThan(T)`, `IsGreaterThanOrEqual(T)`
- `IsLessThan(T)`, `IsLessThanOrEqual(T)`
- `AndIsBetween(T, T)`
- `IsPositive()`, `IsNegative()`, `IsZero()`, `IsNotZero()`

### Object Methods
- `IsNotNull()`, `IsNull()`
- `AndEquals(T)`, `AndNotEquals(T)`
- `AndIsOfType<T, TExpected>()`
- `AndSatisfies(Func<T, bool>, string?)`

### Collection Methods
- `IsNotNull()`, `IsNotEmpty()`, `IsNotNullOrEmpty()`
- `AndHasMinCount(int)`, `AndHasMaxCount(int)`
- `AndHasCountBetween(int, int)`, `AndHasExactCount(int)`
- `AndContains(T)`, `AndDoesNotContain(T)`
- `AndAllItemsSatisfy(predicate)`, `AndAnyItemSatisfies(predicate)`
- `AndContainsNoNulls()`, `AndContainsDistinctItems()`

## Build Status

✅ **All projects build successfully with new guidelines**
- 0 Errors
- 0 Warnings
- Build time: ~27 seconds

## Next Steps

1. Review and approve these changes
2. Communicate to team members
3. Start using in new code immediately
4. Plan optional refactoring of existing code
5. Update team documentation and onboarding materials

## Questions or Issues?

See the comprehensive guide in `docs/VALIDATION_GUIDE.md` or review the implementation in:
- `src/backend/BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/Validation/`
