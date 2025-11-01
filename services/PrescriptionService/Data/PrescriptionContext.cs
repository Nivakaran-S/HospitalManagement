using Microsoft.EntityFrameworkCore;
using PrescriptionService.Models;

namespace PrescriptionService.Data
{
    public class PrescriptionContext : DbContext
    {
        public PrescriptionContext(DbContextOptions<PrescriptionContext> options) : base(options) { }

        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prescription Configuration
            modelBuilder.Entity<Prescription>()
                .HasIndex(p => p.PatientId);

            modelBuilder.Entity<Prescription>()
                .HasIndex(p => p.DoctorId);

            modelBuilder.Entity<Prescription>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Prescription>()
                .HasIndex(p => p.PrescribedDate);

            // PrescriptionMedicine Configuration
            modelBuilder.Entity<PrescriptionMedicine>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.Medicines)
                .HasForeignKey(pm => pm.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PrescriptionMedicine>()
                .HasIndex(pm => pm.PrescriptionId);

            modelBuilder.Entity<PrescriptionMedicine>()
                .HasIndex(pm => pm.IsDispensed);
        }
    }
}