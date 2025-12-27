# ADR-003: IValueObject Marker Interface

**Status:** Accepted
**Date:** 2025-11-16
**Decision Makers:** Architecture Team
**Context:** Domain Layer Type System

---

## Context

The Orange Car Rental codebase contains 40+ value objects across all services (Customers, Fleet, Pricing, Reservations, BuildingBlocks). These value objects represent domain concepts like `Money`, `EmailAddress`, `CustomerName`, `VehicleIdentifier`, etc.

### Problem

Without a common type marker, there was no way to:
- Identify value objects at compile-time for generic constraints
- Discover all value objects via reflection
- Enable framework-level behaviors for value objects
- Document architectural intent in code
- Support tooling and code analyzers

### Current State (Before Decision)

```csharp
// No common interface - just isolated value objects
public readonly record struct Money(decimal NetAmount, decimal VatAmount, Currency Currency) { }
public readonly record struct EmailAddress(string Value) { }
public readonly record struct CustomerName(...) { }
// ... 37 more value objects
```

### Proposed Solution

```csharp
// Marker interface in BuildingBlocks
public interface IValueObject { }

// Applied to all value objects
public readonly record struct Money(...) : IValueObject { }
public readonly record struct EmailAddress(string Value) : IValueObject { }
public readonly record struct CustomerName(...) : IValueObject { }
```

---

## Decision

We will introduce an **empty marker interface `IValueObject`** to identify all value objects in the domain layer.

All value objects across all services will implement this interface to enable:
- Type-safe generic constraints
- Reflection-based discovery
- Framework integration
- Architectural documentation

---

## Rationale

### Advantages

1. **Type Safety via Generic Constraints**
   ```csharp
   public class ValueObjectValidator<T> where T : IValueObject
   {
       public ValidationResult Validate(T valueObject) { ... }
   }
   ```
   - Compile-time verification
   - IDE autocomplete support
   - Prevents incorrect usage

2. **Reflection and Discovery**
   ```csharp
   var valueObjectTypes = Assembly.GetExecutingAssembly()
       .GetTypes()
       .Where(t => typeof(IValueObject).IsAssignableFrom(t));
   ```
   - Runtime introspection
   - Code generation tools
   - Documentation generators

3. **Framework Integration**
   ```csharp
   // Custom JSON converter for all value objects
   public class ValueObjectConverter : JsonConverter<IValueObject>
   {
       // Automatic serialization strategy
   }
   ```
   - Unified serialization
   - Validation frameworks
   - Mapping libraries

4. **Architectural Documentation**
   - Self-documenting code
   - Clear intent: "This is a value object"
   - Easier onboarding for new developers

5. **Code Analysis & Tooling**
   - Roslyn analyzers can enforce patterns
   - IDE warnings for missing implementations
   - Automated refactoring support

### Trade-offs

1. **Empty Interface**
   - No methods or properties
   - **Mitigation:** Purpose is type identification, not behavior

2. **Additional Typing**
   - Must add `: IValueObject` to 40+ types
   - **Mitigation:** One-time effort, long-term benefits

3. **Coupling to BuildingBlocks**
   - All services depend on BuildingBlocks for interface
   - **Mitigation:** Acceptable, BuildingBlocks already central dependency

---

## Implementation Details

### Interface Definition

**Location:** `BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/ValueObjects/IValueObject.cs`

```csharp
namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Marker interface for value objects in the domain.
///     Value objects are immutable objects that are defined by their attributes
///     rather than a unique identity. They implement value equality.
/// </summary>
/// <remarks>
///     Value objects should:
///     - Be immutable (readonly record struct or record)
///     - Implement value-based equality
///     - Contain validation logic in factory methods
///     - Not have an identity (no Id property)
///     - Be replaceable (updating means creating a new instance)
/// </remarks>
public interface IValueObject
{
}
```

### Application Pattern

**BuildingBlocks (4 value objects):**
```csharp
public readonly record struct Money(decimal NetAmount, decimal VatAmount, Currency Currency) : IValueObject { }
public readonly record struct Currency(string Code) : IValueObject { }
public readonly record struct EmailAddress(string Value) : IValueObject { }
public readonly record struct SearchTerm(string Value) : IValueObject { }
```

**Customers Service (10 value objects):**
```csharp
public readonly record struct CustomerIdentifier(Guid Value) : IValueObject { }
public readonly record struct CustomerName(...) : IValueObject { }
public readonly record struct Email(string Value) : IValueObject { }
// ... 7 more
```

**Fleet Service (14 value objects):**
```csharp
public readonly record struct VehicleIdentifier(Guid Value) : IValueObject { }
public readonly record struct VehicleName(string Value) : IValueObject { }
// ... 12 more
```

**Pricing Service (4 value objects):**
```csharp
public readonly record struct PricingPolicyIdentifier(Guid Value) : IValueObject { }
public readonly record struct CategoryCode(string Value) : IValueObject { }
// ... 2 more
```

**Reservations Service (6 value objects):**
```csharp
public readonly record struct ReservationIdentifier(Guid Value) : IValueObject { }
public readonly record struct LocationCode(string Value) : IValueObject { }
// ... 4 more
```

### Usage Examples

#### Generic Validation

```csharp
public interface IValueObjectValidator<T> where T : IValueObject
{
    ValidationResult Validate(T valueObject);
}

public class MoneyValidator : IValueObjectValidator<Money>
{
    public ValidationResult Validate(Money money)
    {
        // Validation logic
    }
}
```

#### Reflection-Based Discovery

```csharp
public static class ValueObjectRegistry
{
    public static IEnumerable<Type> GetAllValueObjects()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IValueObject).IsAssignableFrom(t) && !t.IsInterface);
    }
}
```

#### Custom Serialization

```csharp
public class ValueObjectJsonConverter : JsonConverter<IValueObject>
{
    public override IValueObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Custom deserialization for all value objects
    }

    public override void Write(Utf8JsonWriter writer, IValueObject value, JsonSerializerOptions options)
    {
        // Custom serialization for all value objects
    }
}
```

---

## Consequences

### Positive

- ✅ **Type-Safe Constraints** - Generic methods can constrain to value objects
- ✅ **Discoverability** - Easy to find all value objects via reflection
- ✅ **Framework Integration** - Enable common behaviors across all value objects
- ✅ **Self-Documenting** - Code clearly identifies value objects
- ✅ **Tooling Support** - Better IDE and analyzer support
- ✅ **Consistency** - Standardized approach across all services

### Negative

- ⚠️ **Empty Interface** - No runtime behavior (by design)
- ⚠️ **One-Time Effort** - Required updating 40 types (completed)

### Neutral

- 📝 Marker interface pattern is well-established (ISerializable, IDisposable, etc.)
- 📝 Adds one more interface to domain model (acceptable complexity)

---

## Alternatives Considered

### 1. No Marker Interface

**Rejected because:**
- No way to identify value objects programmatically
- Cannot use generic constraints
- Missed opportunity for framework integration
- Less clear architectural intent

### 2. Base Class Instead of Interface

```csharp
public abstract record ValueObject { }
public record struct Money(...) : ValueObject { } // ❌ Structs cannot inherit classes
```

**Rejected because:**
- C# structs cannot inherit from classes
- Many value objects use `record struct` for performance
- Less flexible than interface

### 3. Attribute-Based Marking

```csharp
[ValueObject]
public readonly record struct Money(...) { }
```

**Not chosen because:**
- Attributes are runtime-only (reflection required)
- Cannot use in generic constraints
- Less discoverable in IDE
- Attributes better suited for metadata, not type identification

### 4. Naming Convention Only

**Rejected because:**
- No compile-time enforcement
- Easy to miss in code reviews
- Tooling cannot reliably detect
- Not self-documenting

---

## Impact on Existing Code

### Files Modified

**Total:** 41 files (1 new interface + 40 value objects)

- BuildingBlocks: 5 files (1 new + 4 updated)
- Customers: 10 files
- Fleet: 14 files
- Pricing: 4 files
- Reservations: 6 files
- Tests: 0 files (no breaking changes)

### Breaking Changes

**None** - Adding an interface to existing types is backward compatible.

### Test Impact

- All 453 tests pass without modification
- No behavioral changes to value objects
- Only type metadata changed

---

## Related Patterns

### Similar Marker Interfaces in Codebase

We already use marker interfaces for other domain concepts:

```csharp
// CQRS markers
public interface ICommand { }
public interface IQuery<TResult> { }

// Domain events
public interface IDomainEvent { }
```

The `IValueObject` marker interface follows this established pattern.

### Future Enhancements

Consider adding similar markers for:
- `IEntity` - Mark entities with identity
- `IAggregateRoot` - Mark aggregate roots
- `ISpecification` - Mark domain specifications

---

## References

- **Domain-Driven Design** - Eric Evans (Value Objects chapter)
- **Implementing Domain-Driven Design** - Vaughn Vernon
- **Marker Interface Pattern** - Gang of Four Design Patterns
- **C# Interface Design** - Microsoft Framework Design Guidelines

---

## Review

This ADR should be reviewed in **6 months** (May 2026) to assess:
- Usage in generic constraints
- Framework integration benefits realized
- Developer feedback on pattern
- Whether additional marker interfaces would be beneficial

---

## Updates

| Date | Change | Author |
|------|--------|--------|
| 2025-11-16 | Initial decision and implementation | Architecture Team |
| 2025-11-16 | Applied to all 40 value objects across services | Architecture Team |
