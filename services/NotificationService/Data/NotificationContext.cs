using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data
{
    public class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Notification Configuration
            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.RecipientId);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.Status);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.RecipientId, n.IsRead });

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.ScheduledFor);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.SentAt);

            // NotificationTemplate Configuration
            modelBuilder.Entity<NotificationTemplate>()
                .HasIndex(nt => nt.TemplateName);

            modelBuilder.Entity<NotificationTemplate>()
                .HasIndex(nt => nt.NotificationType);

            // NotificationPreference Configuration
            modelBuilder.Entity<NotificationPreference>()
                .HasIndex(np => new { np.UserId, np.UserType })
                .IsUnique();

            // NotificationLog Configuration
            modelBuilder.Entity<NotificationLog>()
                .HasOne(nl => nl.Notification)
                .WithMany()
                .HasForeignKey(nl => nl.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationLog>()
                .HasIndex(nl => nl.NotificationId);

            modelBuilder.Entity<NotificationLog>()
                .HasIndex(nl => nl.Timestamp);
        }
    }
}