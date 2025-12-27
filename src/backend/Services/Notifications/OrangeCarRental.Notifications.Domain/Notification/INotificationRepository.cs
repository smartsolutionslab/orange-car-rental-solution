using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

/// <summary>
///     Repository interface for Notification aggregate.
///     Provides access to notifications in the persistence layer.
/// </summary>
public interface INotificationRepository : IRepository<Notification, NotificationIdentifier>
{
    // All common repository methods (GetByIdAsync, AddAsync, UpdateAsync, RemoveAsync, SaveChangesAsync)
    // are inherited from IRepository<Notification, NotificationIdentifier>

    // Add domain-specific query methods here if needed in the future, for example:
    // Task<IEnumerable<Notification>> GetPendingNotificationsAsync(CancellationToken ct = default);
    // Task<IEnumerable<Notification>> GetNotificationsByStatusAsync(NotificationStatus status, CancellationToken ct = default);
}
