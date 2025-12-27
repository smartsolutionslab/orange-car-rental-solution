# ADR-001: Immutable Aggregate Pattern

**Status:** Accepted
**Date:** 2025-11-15
**Decision Makers:** Architecture Team
**Context:** Domain Layer Architecture

---

## Context

Domain aggregates (Customer, Vehicle, Reservation, PricingPolicy) require a pattern for state modifications that ensures consistency, thread-safety, and clear intent.

### Traditional Approach (Mutable Aggregates)

```csharp
public class Customer
{
    public CustomerName Name { get; private set; }

    public void UpdateProfile(CustomerName name, ...)
    {
        Name = name;  // Mutates existing instance
        // Raise domain event
    }
}
```

### Alternative Approach (Immutable Aggregates)

```csharp
public class Customer
{
    public CustomerName Name { get; init; }  // init-only

    public Customer UpdateProfile(CustomerName name, ...)
    {
        var updated = CreateMutatedCopy(name, ...);  // Returns new instance
        updated.AddDomainEvent(new CustomerProfileUpdated(...));
        return updated;
    }
}
```

---

## Decision

We will use **immutable aggregates** with `init`-only properties and methods that return new instances.

---

## Rationale

### Advantages

1. **Functional Programming Benefits**
   - Easier to reason about state changes
   - No hidden side effects
   - Thread-safe by default

2. **Clear Intent**
   - Method signatures explicitly show state change: `Customer UpdateProfile(...)` returns new `Customer`
   - Calling code must handle the returned instance

3. **Testability**
   - Easy to verify state transitions
   - No need to track mutable state over time

4. **Accidental Mutation Prevention**
   - `init`-only properties prevent accidental modification
   - Compile-time enforcement

5. **Audit Trail**
   - Each state change creates new instance with timestamp
   - Can track object history if needed

### Trade-offs

1. **Memory Overhead**
   - Creates new object on every change
   - **Mitigation:** Aggregates are small, not performance-critical

2. **Repository Complexity**
   - Infrastructure must handle instance replacement
   - **Mitigation:** EF Core handles this with proper tracking

3. **Non-Standard Pattern**
   - Most DDD examples use mutable aggregates
   - **Mitigation:** Well-documented in codebase

4. **Learning Curve**
   - New developers may find pattern unfamiliar
   - **Mitigation:** Clear documentation and examples

---

## Implementation Details

### Aggregate Base Class

```csharp
public abstract class AggregateRoot<TId> : Entity<TId>
{
    // Properties are init-only
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }

    // Methods return new instances
    protected TAggregate CreateMutatedCopy<TAggregate>(...)
    {
        // Creates new instance with updated values
    }
}
```

### Repository Pattern

```csharp
public async Task<Customer> UpdateAsync(Customer customer, ...)
{
    // EF Core tracks and updates the entity
    context.Customers.Update(customer);
    await context.SaveChangesAsync(cancellationToken);
    return customer;
}
```

### Usage in Handlers

```csharp
public async Task<UpdateCustomerProfileResult> HandleAsync(...)
{
    var customer = await customers.GetByIdAsync(command.CustomerId, ...);

    // Returns NEW instance
    var updated = customer.UpdateProfile(command.Name, command.PhoneNumber, ...);

    // Repository handles replacement
    await customers.UpdateAsync(updated, ...);
    await customers.SaveChangesAsync(...);

    return new UpdateCustomerProfileResult { ... };
}
```

---

## Consequences

### Positive

- ✅ Thread-safe domain model
- ✅ Clear state change semantics
- ✅ Prevents accidental mutations
- ✅ Easier to test
- ✅ Functional programming style

### Negative

- ⚠️ Slight memory overhead (acceptable for business objects)
- ⚠️ Requires team training on pattern
- ⚠️ EF Core tracking must be handled correctly

### Neutral

- 📝 Different from typical DDD examples (but valid approach)
- 📝 Requires documentation for maintainability

---

## Alternatives Considered

### 1. Traditional Mutable Aggregates

**Rejected because:**
- Allows accidental mutations
- Harder to reason about state
- No compile-time protection

### 2. Event Sourcing

**Not chosen because:**
- Adds significant complexity
- Not required for current business needs
- Can be added later if needed

### 3. Partial Immutability (Private Setters)

**Not chosen because:**
- Still allows mutations within aggregate
- Less explicit about state changes
- No functional programming benefits

---

## References

- **Functional Domain Modeling** - Scott Wlaschin
- **Domain Modeling Made Functional** - Scott Wlaschin
- **C# 9 Records** - Microsoft Documentation
- **Immutable Objects in C#** - Martin Fowler

---

## Review

This ADR should be reviewed in **6 months** (May 2026) to assess:
- Developer satisfaction with pattern
- Performance impact (if any)
- Maintenance burden
- Whether to continue, modify, or abandon approach

---

## Updates

| Date | Change | Author |
|------|--------|--------|
| 2025-11-15 | Initial decision | Architecture Team |
