using Microsoft.EntityFrameworkCore;
using StaffService.Models;

namespace StaffService.Data
{
    public class StaffContext : DbContext
    {
        public StaffContext(DbContextOptions<StaffContext> options) : base(options) { }
        
        public DbSet<Staff> StaffMembers { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }
        public DbSet<StaffRolePermission> StaffRolePermissions { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Staff Configuration
            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.KeycloakUserId)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Email);

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Department);

            // StaffAttendance Configuration
            modelBuilder.Entity<StaffAttendance>()
                .HasOne(sa => sa.Staff)
                .WithMany(s => s.Attendances)
                .HasForeignKey(sa => sa.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffAttendance>()
                .HasIndex(sa => new { sa.StaffId, sa.Date })
                .IsUnique();

            // StaffLeave Configuration
            modelBuilder.Entity<StaffLeave>()
                .HasOne(sl => sl.Staff)
                .WithMany(s => s.Leaves)
                .HasForeignKey(sl => sl.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffLeave>()
                .HasIndex(sl => sl.StaffId);

            modelBuilder.Entity<StaffLeave>()
                .HasIndex(sl => sl.Status);
        }
    }
}