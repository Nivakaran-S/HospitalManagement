using Microsoft.EntityFrameworkCore;
using PatientService.Models;

namespace PatientService.Data
{
    public class PatientContext : DbContext
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options) { }
        
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Patient Configuration
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.KeycloakUserId)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Email);

            // MedicalRecord Configuration
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MedicalRecord>()
                .HasIndex(m => m.PatientId);

            modelBuilder.Entity<MedicalRecord>()
                .HasIndex(m => m.VisitDate);

            // VitalSign Configuration
            modelBuilder.Entity<VitalSign>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.VitalSigns)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VitalSign>()
                .HasIndex(v => v.PatientId);

            modelBuilder.Entity<VitalSign>()
                .HasIndex(v => v.RecordedAt);
        }
    }
}