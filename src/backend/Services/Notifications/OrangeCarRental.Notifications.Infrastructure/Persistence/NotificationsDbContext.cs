using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;
using SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence;

/// <summary>
///     Database context for the Notifications service.
///     Manages notification data and delivery tracking.
/// </summary>
public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new NotificationConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("notifications");
    }
}
