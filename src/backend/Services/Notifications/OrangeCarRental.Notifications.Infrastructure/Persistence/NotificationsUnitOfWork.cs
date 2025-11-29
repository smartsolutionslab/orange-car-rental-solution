using SmartSolutionsLab.OrangeCarRental.Notifications.Domain;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Notifications bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class NotificationsUnitOfWork : INotificationsUnitOfWork
{
    private readonly NotificationsDbContext _context;
    private INotificationRepository? _notifications;

    public NotificationsUnitOfWork(NotificationsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public INotificationRepository Notifications =>
        _notifications ??= new NotificationRepository(_context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
