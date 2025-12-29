using Eventuous.Subscriptions;
using Eventuous.Subscriptions.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Projections;

/// <summary>
/// Projection that subscribes to Customer domain events and updates the EF Core read model.
/// This keeps the existing database tables in sync with the event store.
/// </summary>
public sealed class CustomerReadModelProjection(IServiceScopeFactory scopeFactory) : IEventHandler
{
    public string DiagnosticName => "CustomerReadModelProjection";

    public async ValueTask<EventHandlingStatus> HandleEvent(IMessageConsumeContext context)
    {
        var result = context.Message switch
        {
            CustomerRegistered e => await HandleAsync(e, context.CancellationToken),
            CustomerProfileUpdated e => await HandleAsync(e, context.CancellationToken),
            CustomerStatusChanged e => await HandleAsync(e, context.CancellationToken),
            DriversLicenseUpdated e => await HandleAsync(e, context.CancellationToken),
            CustomerEmailChanged e => await HandleAsync(e, context.CancellationToken),
            _ => EventHandlingStatus.Ignored
        };

        return result;
    }

    private async Task<EventHandlingStatus> HandleAsync(CustomerRegistered @event, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();

        // Create a new customer aggregate and apply the event
        var customer = new Customer();
        customer.Load([@event]);

        // Add to database
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(CustomerProfileUpdated @event, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == @event.CustomerIdentifier, cancellationToken);

        if (customer is null)
            return EventHandlingStatus.Ignored;

        // Load the event to update state
        customer.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(CustomerStatusChanged @event, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == @event.CustomerIdentifier, cancellationToken);

        if (customer is null)
            return EventHandlingStatus.Ignored;

        customer.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(DriversLicenseUpdated @event, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == @event.CustomerIdentifier, cancellationToken);

        if (customer is null)
            return EventHandlingStatus.Ignored;

        customer.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(CustomerEmailChanged @event, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == @event.CustomerIdentifier, cancellationToken);

        if (customer is null)
            return EventHandlingStatus.Ignored;

        customer.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }
}
