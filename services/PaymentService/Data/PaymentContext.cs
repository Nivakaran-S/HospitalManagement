using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Data
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentRefund> PaymentRefunds { get; set; }
        public DbSet<PaymentGatewayConfig> PaymentGatewayConfigs { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Payment Configuration
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PatientId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.TransactionId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PaymentDate);

            // PaymentRefund Configuration
            modelBuilder.Entity<PaymentRefund>()
                .HasOne(pr => pr.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(pr => pr.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentRefund>()
                .HasIndex(pr => pr.PaymentId);

            // PaymentGatewayConfig Configuration
            modelBuilder.Entity<PaymentGatewayConfig>()
                .HasIndex(pgc => pgc.GatewayName);

            // InsuranceClaim Configuration
            modelBuilder.Entity<InsuranceClaim>()
                .HasIndex(ic => ic.PatientId);

            modelBuilder.Entity<InsuranceClaim>()
                .HasIndex(ic => ic.ClaimNumber)
                .IsUnique();

            modelBuilder.Entity<InsuranceClaim>()
                .HasIndex(ic => ic.Status);
        }
    }
}