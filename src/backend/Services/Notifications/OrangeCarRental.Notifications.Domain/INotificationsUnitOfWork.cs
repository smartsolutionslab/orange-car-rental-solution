using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Domain;

/// <summary>
///     Unit of Work for the Notifications bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface INotificationsUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the notification repository.
    /// </summary>
    INotificationRepository Notifications { get; }
}
