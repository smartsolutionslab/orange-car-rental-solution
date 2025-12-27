using Eventuous.Subscriptions;
using Eventuous.Subscriptions.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Projections;

/// <summary>
/// Projection that subscribes to Reservation domain events and updates the EF Core read model.
/// This keeps the existing database tables in sync with the event store.
/// </summary>
public sealed class ReservationReadModelProjection : IEventHandler
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationReadModelProjection(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public string DiagnosticName => "ReservationReadModelProjection";

    public async ValueTask<EventHandlingStatus> HandleEvent(IMessageConsumeContext context)
    {
        var result = context.Message switch
        {
            ReservationCreated e => await HandleAsync(e, context.CancellationToken),
            ReservationConfirmed e => await HandleAsync(e, context.CancellationToken),
            ReservationCancelled e => await HandleAsync(e, context.CancellationToken),
            ReservationActivated e => await HandleAsync(e, context.CancellationToken),
            ReservationCompleted e => await HandleAsync(e, context.CancellationToken),
            ReservationMarkedNoShow e => await HandleAsync(e, context.CancellationToken),
            _ => EventHandlingStatus.Ignored
        };

        return result;
    }

    private async Task<EventHandlingStatus> HandleAsync(ReservationCreated @event, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();

        // Create a new reservation aggregate and apply the event
        var reservation = new Reservation();
        reservation.Load([@event]);

        // Add to database
        dbContext.Reservations.Add(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(ReservationConfirmed @event, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();

        var reservation = await dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == @event.ReservationId, cancellationToken);

        if (reservation is null)
            return EventHandlingStatus.Ignored;

        reservation.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(ReservationCancelled @event, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();

        var reservation = await dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == @event.ReservationId, cancellationToken);

        if (reservation is null)
            return EventHandlingStatus.Ignored;

        reservation.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(ReservationActivated @event, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();

        var reservation = await dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == @event.ReservationId, cancellationToken);

        if (reservation is null)
            return EventHandlingStatus.Ignored;

        reservation.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(ReservationCompleted @event, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();

        var reservation = await dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == @event.ReservationId, cancellationToken);

        if (reservation is null)
            return EventHandlingStatus.Ignored;

        reservation.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }

    private async Task<EventHandlingStatus> HandleAsync(ReservationMarkedNoShow @event, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();

        var reservation = await dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == @event.ReservationId, cancellationToken);

        if (reservation is null)
            return EventHandlingStatus.Ignored;

        reservation.Load([@event]);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EventHandlingStatus.Success;
    }
}
