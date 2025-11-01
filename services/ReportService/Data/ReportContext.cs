using Microsoft.EntityFrameworkCore;
using ReportService.Models;

namespace ReportService.Data
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options) : base(options) { }

        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportTemplate> ReportTemplates { get; set; }
        public DbSet<ScheduledReport> ScheduledReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Report Configuration
            modelBuilder.Entity<Report>()
                .HasIndex(r => r.ReportType);

            modelBuilder.Entity<Report>()
                .HasIndex(r => r.GeneratedDate);

            modelBuilder.Entity<Report>()
                .HasIndex(r => r.Status);

            // ReportTemplate Configuration
            modelBuilder.Entity<ReportTemplate>()
                .HasIndex(rt => rt.TemplateName);

            modelBuilder.Entity<ReportTemplate>()
                .HasIndex(rt => rt.ReportType);

            // ScheduledReport Configuration
            modelBuilder.Entity<ScheduledReport>()
                .HasOne(sr => sr.ReportTemplate)
                .WithMany()
                .HasForeignKey(sr => sr.ReportTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScheduledReport>()
                .HasIndex(sr => sr.NextRunDate);

            modelBuilder.Entity<ScheduledReport>()
                .HasIndex(sr => sr.IsActive);
        }
    }
}