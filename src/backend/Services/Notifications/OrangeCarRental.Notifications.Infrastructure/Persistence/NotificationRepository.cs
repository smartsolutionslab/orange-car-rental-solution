using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence;

/// <summary>
///     Entity Framework implementation of INotificationRepository.
/// </summary>
public sealed class NotificationRepository(NotificationsDbContext context) : INotificationRepository
{
    public async Task<Notification> GetByIdAsync(NotificationIdentifier id, CancellationToken cancellationToken = default)
    {
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        return notification ?? throw new EntityNotFoundException(typeof(Notification), id);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default) =>
        await context.Notifications.AddAsync(notification, cancellationToken);

    public Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        context.Notifications.Update(notification);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        context.Notifications.Remove(notification);
        return Task.CompletedTask;
    }
}
