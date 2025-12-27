# ADR-002: Direct Handler Injection (No MediatR)

**Status:** Accepted
**Date:** 2025-11-15
**Decision Makers:** Architecture Team
**Context:** CQRS Implementation

---

## Context

The application uses CQRS pattern with commands and queries. We need to decide how to dispatch these to their handlers.

### Option 1: MediatR Library

```csharp
// With MediatR
public async Task<IResult> CreateReservation(
    CreateReservationRequest request,
    IMediator mediator,  // Inject mediator
    CancellationToken cancellationToken)
{
    var command = new CreateReservationCommand(...);
    var result = await mediator.Send(command, cancellationToken);  // Mediator finds handler
    return Results.Created(...);
}
```

### Option 2: Direct Handler Injection

```csharp
// Without MediatR
public async Task<IResult> CreateReservation(
    CreateReservationRequest request,
    CreateReservationCommandHandler handler,  // Inject specific handler
    CancellationToken cancellationToken)
{
    var command = new CreateReservationCommand(...);
    var result = await handler.HandleAsync(command, cancellationToken);  // Direct call
    return Results.Created(...);
}
```

---

## Decision

We will use **direct handler injection** without MediatR.

---

## Rationale

### Advantages of Direct Injection

1. **Explicit Dependencies**
   - Clear what handlers are needed
   - Easy to see dependencies at compile time
   - Better IDE support (Go to Definition works perfectly)

2. **Simpler Mental Model**
   - No "magic" - handler is directly injected
   - Easier to understand for junior developers
   - No reflection or assembly scanning

3. **Faster Startup Time**
   - No assembly scanning for handlers
   - No registration overhead
   - Immediate application startup

4. **Better Debugging**
   - Stack traces are clearer
   - Can step directly into handler
   - No mediator pipeline to navigate

5. **Zero External Dependencies**
   - No NuGet package required
   - Less maintenance burden
   - No version conflicts

6. **Performance**
   - No mediator overhead
   - Direct method call (faster)
   - No boxing/unboxing

### Trade-offs

1. **Manual Registration**
   - Must register each handler in DI container
   - More boilerplate code in `Program.cs`
   - **Mitigation:** Acceptable for this codebase size (12 commands, 9 queries)

2. **No Cross-Cutting Behaviors**
   - Cannot add logging/validation pipeline automatically
   - **Mitigation:** Can use decorators if needed

3. **No Dynamic Handler Discovery**
   - Handlers must be explicitly registered
   - **Mitigation:** Registration is clear and visible

---

## When to Reconsider

Consider switching to MediatR if:

1. **Codebase grows significantly** (100+ handlers)
   - Manual registration becomes burdensome
   - Need automatic handler discovery

2. **Cross-cutting behaviors needed**
   - Logging every command
   - Validation pipeline
   - Transaction management
   - Retry policies

3. **Multiple handlers per command**
   - Notification pattern (multiple handlers for one command)
   - Currently not needed

4. **Team grows**
   - Need stronger conventions
   - Want to enforce patterns automatically

---

## Implementation Details

### Handler Registration

```csharp
// Program.cs
builder.Services.AddScoped<RegisterCustomerCommandHandler>();
builder.Services.AddScoped<UpdateCustomerProfileCommandHandler>();
builder.Services.AddScoped<GetCustomerQueryHandler>();
// ... (21 total registrations)
```

### Endpoint Example

```csharp
customers.MapPost("/", async (
    RegisterCustomerRequest request,
    RegisterCustomerCommandHandler handler,  // Specific handler
    CancellationToken cancellationToken) =>
{
    var command = new RegisterCustomerCommand(...);
    var result = await handler.HandleAsync(command, cancellationToken);
    return Results.Created($"/api/customers/{result.CustomerId}", result);
});
```

---

## Alternative: Decorator Pattern for Cross-Cutting Concerns

If cross-cutting behaviors are needed, we can use decorators instead of MediatR:

```csharp
public class LoggingCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    ILogger<TCommand> logger)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken ct)
    {
        logger.LogInformation("Executing {CommandName}", typeof(TCommand).Name);
        try
        {
            var result = await inner.HandleAsync(command, ct);
            logger.LogInformation("Executed {CommandName}", typeof(TCommand).Name);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute {CommandName}", typeof(TCommand).Name);
            throw;
        }
    }
}

// Registration with decorator
builder.Services.AddScoped<RegisterCustomerCommandHandler>();
builder.Services.Decorate<ICommandHandler<RegisterCustomerCommand, RegisterCustomerResult>,
                         LoggingCommandHandlerDecorator<RegisterCustomerCommand, RegisterCustomerResult>>();
```

---

## Comparison: Direct Injection vs MediatR

| Aspect | Direct Injection | MediatR |
|--------|------------------|---------|
| **Complexity** | Low | Medium |
| **Explicitness** | High | Low (magic) |
| **IDE Support** | Excellent | Good |
| **Debugging** | Easy | Harder |
| **Startup Time** | Fast | Slower (scanning) |
| **Boilerplate** | More registration | Less registration |
| **Cross-Cutting** | Manual decorators | Built-in pipeline |
| **Dependencies** | Zero | 1 NuGet package |
| **Performance** | Faster | Slightly slower |
| **Learning Curve** | Easier | Harder |

---

## Decision Factors

For our codebase (21 handlers total):

✅ **Simplicity is more important than automation**
✅ **Explicitness is more important than convenience**
✅ **Zero dependencies is valuable**
✅ **Better debugging outweighs registration boilerplate**

---

## Migration Path (If Needed)

If we decide to add MediatR later:

1. **Phase 1:** Add MediatR NuGet package
2. **Phase 2:** Register all handlers with MediatR
3. **Phase 3:** Update endpoints to use `IMediator`
4. **Phase 4:** Remove manual registrations
5. **Phase 5:** Add pipeline behaviors (logging, validation, etc.)

**Effort:** 1-2 days
**Risk:** Low (handlers already implement correct interfaces)

---

## Consequences

### Positive

- ✅ Simpler codebase
- ✅ Explicit dependencies
- ✅ Better debugging
- ✅ Faster startup
- ✅ Zero external dependencies

### Negative

- ⚠️ Manual handler registration (21 lines)
- ⚠️ No built-in pipeline behaviors
- ⚠️ Cross-cutting concerns require decorators

### Neutral

- 📝 Different from many CQRS examples (most use MediatR)
- 📝 Requires documentation for new developers

---

## References

- **CQRS Journey** - Microsoft Patterns & Practices
- **Vertical Slice Architecture** - Jimmy Bogard (MediatR author, but acknowledges simpler approaches)
- **Simple Made Easy** - Rich Hickey (simplicity vs convenience)

---

## Review

This ADR should be reviewed when:
- Handler count exceeds 50 (currently 21)
- Team requests cross-cutting behaviors
- Multiple developers report registration pain
- Integration with external CQRS infrastructure needed

**Next review:** When handler count reaches 50

---

## Updates

| Date | Change | Author |
|------|--------|--------|
| 2025-11-15 | Initial decision | Architecture Team |
