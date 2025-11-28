using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence.Configurations;

/// <summary>
///     Entity configuration for Notification aggregate.
/// </summary>
internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        // Primary key - NotificationIdentifier struct with Guid value
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id)
            .HasColumnName("NotificationId")
            .HasConversion(
                id => id.Value,
                value => NotificationIdentifier.From(value))
            .IsRequired();

        // NotificationType enum - stored as string
        builder.Property(n => n.Type)
            .HasColumnName("Type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // RecipientEmail value object (nullable - only for email notifications)
        builder.Property(n => n.RecipientEmail)
            .HasColumnName("RecipientEmail")
            .HasConversion(
                email => email.HasValue ? email.Value.Value : null,
                value => value != null ? RecipientEmail.From(value) : null)
            .HasMaxLength(255);

        // RecipientPhone value object (nullable - only for SMS notifications)
        builder.Property(n => n.RecipientPhone)
            .HasColumnName("RecipientPhone")
            .HasConversion(
                phone => phone.HasValue ? phone.Value.Value : null,
                value => value != null ? RecipientPhone.From(value) : null)
            .HasMaxLength(20);

        // NotificationSubject value object
        builder.Property(n => n.Subject)
            .HasColumnName("Subject")
            .HasConversion(
                subject => subject.Value,
                value => NotificationSubject.From(value))
            .HasMaxLength(200)
            .IsRequired();

        // NotificationContent value object
        builder.Property(n => n.Content)
            .HasColumnName("Content")
            .HasConversion(
                content => content.Value,
                value => NotificationContent.From(value))
            .HasMaxLength(10000)
            .IsRequired();

        // NotificationStatus enum - stored as string
        builder.Property(n => n.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Provider message ID (nullable)
        builder.Property(n => n.ProviderMessageId)
            .HasColumnName("ProviderMessageId")
            .HasMaxLength(255);

        // Error message (nullable)
        builder.Property(n => n.ErrorMessage)
            .HasColumnName("ErrorMessage")
            .HasMaxLength(1000);

        // Timestamps
        builder.Property(n => n.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(n => n.SentAt)
            .HasColumnName("SentAt");

        builder.Property(n => n.DeliveredAt)
            .HasColumnName("DeliveredAt");

        // Ignore domain events (not persisted)
        builder.Ignore(n => n.DomainEvents);

        // Indexes for common queries
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.Type);
        builder.HasIndex(n => n.RecipientEmail);
        builder.HasIndex(n => n.RecipientPhone);
        builder.HasIndex(n => n.CreatedAt);
    }
}
