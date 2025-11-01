using Microsoft.EntityFrameworkCore;
using AppointmentService.Models;

namespace AppointmentService.Data
{
    public class AppointmentContext : DbContext
    {
        public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Appointment Configuration
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.DoctorId);

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.AppointmentDate, a.AppointmentTime });

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.Status);

            // Unique constraint to prevent double booking
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.AppointmentTime })
                .IsUnique()
                .HasFilter("[Status] != 'Cancelled'");
        }
    }
}