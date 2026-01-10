# Claude Code Project Instructions

## Project: Orange Car Rental

A car rental platform built with .NET 9 microservices (backend) and Angular 21 (frontend).

## Coding Style Rules

### Single-Line If Statements (CRITICAL)

**Always prefer single-line if statements without brackets when:**
- There is no `else` clause
- The statement fits on one line
- It's a guard clause, early return, or simple operation

```csharp
// PREFERRED - Backend (C#)
if (value is null) return;
if (count == 0) throw new InvalidOperationException();
if (!IsValid()) return null;
if (customer is null) throw new ArgumentNullException(nameof(customer));
if (existingLocation != null) throw new InvalidOperationException($"Location exists.");
```

```typescript
// PREFERRED - Frontend (TypeScript/Angular)
if (!isValid) return;
if (items.length === 0) return [];
if (this.loading()) return;
if (!hasRequiredRole) return false;
```

**Only use brackets when:**
- Line is too long and needs breaking
- Multiple statements inside the if block
- There is an else clause

```csharp
// Brackets needed - multiple statements
if (condition)
{
    DoSomething();
    DoSomethingElse();
}

// Brackets needed - line too long
if (someVeryLongConditionThatRequiresLineBreaking && anotherCondition)
{
    throw new InvalidOperationException("Error message");
}
```

### Validation with Ensure.That() (CRITICAL)

**Always use `Ensure.That()` for:**
- Parameter validation
- Null checks
- Dependency checks (constructor injection)
- Any place where exceptions are thrown for validation

```csharp
// PREFERRED - Use Ensure.That()
Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();
Ensure.That(email, nameof(email)).IsNotNullOrWhiteSpace().AndIsValidEmail();
Ensure.That(count, nameof(count)).IsGreaterThan(0);
Ensure.That(items, nameof(items)).IsNotNullOrEmpty();

// For GUID identifiers - use IsNotEmpty()
Ensure.That(id, nameof(id)).IsNotEmpty();

// AVOID - Old style
ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
if (value == null) throw new ArgumentNullException(nameof(value));
```

**Ensure.That() namespace:**
```csharp
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
```

### Other Style Preferences

- Use `var` when type is apparent
- Private fields: camelCase without underscore prefix
- Expression-bodied members for single-line methods/properties
- Pattern matching over type checks

## Architecture

### Backend Structure
- Domain-Driven Design with microservices
- Immutable aggregates (record types)
- CQRS pattern (without MediatR)
- Value objects with factory methods

### Frontend Structure
- Angular 21 with standalone components
- Signal-based state management
- Tailwind CSS for styling
- Monorepo with shared libs (@orange-car-rental/*)

## Key Files

- `.editorconfig` - Code style enforcement
- `docs/CODING_GUIDELINES_CHANGES.md` - Detailed coding guidelines
- `docs/VALIDATION_GUIDE.md` - Ensure.That() usage guide
- `src/backend/docs/ADR-*.md` - Architecture Decision Records

## Do NOT

- Add unnecessary comments or documentation unless asked
- Create new files when editing existing ones would work
- Use brackets for simple single-line if statements
- Use old-style validation (ArgumentException.ThrowIfNull, etc.)
- Add emojis unless explicitly requested
