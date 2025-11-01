using Microsoft.EntityFrameworkCore;
using DoctorService.Models;

namespace DoctorService.Data
{
    public class DoctorContext : DbContext
    {
        public DoctorContext(DbContextOptions<DoctorContext> options) : base(options) { }
        
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<DoctorLeave> DoctorLeaves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Doctor Configuration
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.KeycloakUserId)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Email);

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Speciality);

            // DoctorAvailability Configuration
            modelBuilder.Entity<DoctorAvailability>()
                .HasOne(da => da.Doctor)
                .WithMany(d => d.Availabilities)
                .HasForeignKey(da => da.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorAvailability>()
                .HasIndex(da => new { da.DoctorId, da.DayOfWeek });

            // DoctorLeave Configuration
            modelBuilder.Entity<DoctorLeave>()
                .HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(dl => dl.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorLeave>()
                .HasIndex(dl => dl.DoctorId);
        }
    }
}