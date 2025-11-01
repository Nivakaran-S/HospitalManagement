using Microsoft.EntityFrameworkCore;
using LabService.Models;

namespace LabService.Data
{
    public class LabTestContext : DbContext
    {
        public LabTestContext(DbContextOptions<LabTestContext> options) : base(options) { }

        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<LabTestResult> LabTestResults { get; set; }
        public DbSet<LabTestTemplate> LabTestTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LabTest Configuration
            modelBuilder.Entity<LabTest>()
                .HasIndex(lt => lt.PatientId);

            modelBuilder.Entity<LabTest>()
                .HasIndex(lt => lt.DoctorId);

            modelBuilder.Entity<LabTest>()
                .HasIndex(lt => lt.Status);

            modelBuilder.Entity<LabTest>()
                .HasIndex(lt => lt.OrderedDate);

            modelBuilder.Entity<LabTest>()
                .HasIndex(lt => lt.TestCategory);

            // LabTestResult Configuration
            modelBuilder.Entity<LabTestResult>()
                .HasOne(ltr => ltr.LabTest)
                .WithMany(lt => lt.TestResults)
                .HasForeignKey(ltr => ltr.LabTestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LabTestResult>()
                .HasIndex(ltr => ltr.LabTestId);

            // LabTestTemplate Configuration
            modelBuilder.Entity<LabTestTemplate>()
                .HasIndex(ltt => ltt.TestName);

            modelBuilder.Entity<LabTestTemplate>()
                .HasIndex(ltt => ltt.TestCategory);
        }
    }
}