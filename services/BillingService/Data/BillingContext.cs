using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data
{
    public class BillingContext : DbContext
    {
        public BillingContext(DbContextOptions<BillingContext> options) : base(options) { }

        public DbSet<Billing> Billings { get; set; }
        public DbSet<BillingItem> BillingItems { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Billing Configuration
            modelBuilder.Entity<Billing>()
                .HasIndex(b => b.PatientId);

            modelBuilder.Entity<Billing>()
                .HasIndex(b => b.Status);

            modelBuilder.Entity<Billing>()
                .HasIndex(b => b.BillingDate);

            // BillingItem Configuration
            modelBuilder.Entity<BillingItem>()
                .HasOne(bi => bi.Billing)
                .WithMany(b => b.BillingItems)
                .HasForeignKey(bi => bi.BillingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BillingItem>()
                .HasIndex(bi => bi.BillingId);

            // PaymentTransaction Configuration
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Billing)
                .WithMany(b => b.PaymentTransactions)
                .HasForeignKey(pt => pt.BillingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
                .HasIndex(pt => pt.BillingId);

            modelBuilder.Entity<PaymentTransaction>()
                .HasIndex(pt => pt.PaymentDate);
        }
    }
}