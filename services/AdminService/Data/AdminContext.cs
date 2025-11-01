using Microsoft.EntityFrameworkCore;
using AdminService.Models;

namespace AdminService.Data
{
    public class AdminContext : DbContext
    {
        public AdminContext(DbContextOptions<AdminContext> options) : base(options) { }
        
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin Configuration
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.KeycloakUserId)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email);

            // SystemLog Configuration
            modelBuilder.Entity<SystemLog>()
                .HasIndex(sl => sl.Timestamp);

            modelBuilder.Entity<SystemLog>()
                .HasIndex(sl => sl.UserId);

            modelBuilder.Entity<SystemLog>()
                .HasIndex(sl => sl.Action);

            // SystemSettings Configuration
            modelBuilder.Entity<SystemSettings>()
                .HasIndex(ss => ss.SettingKey)
                .IsUnique();
        }
    }
}