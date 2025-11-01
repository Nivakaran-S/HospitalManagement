using Microsoft.EntityFrameworkCore;
using DonorService.Models;

namespace DonorService.Data
{
    public class DonorContext : DbContext
    {
        public DonorContext(DbContextOptions<DonorContext> options) : base(options) { }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<DonationRecord> DonationRecords { get; set; }
        public DbSet<BloodInventory> BloodInventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Donor Configuration
            modelBuilder.Entity<Donor>()
                .HasIndex(d => d.Email);

            modelBuilder.Entity<Donor>()
                .HasIndex(d => d.BloodGroup);

            modelBuilder.Entity<Donor>()
                .HasIndex(d => d.ContactNumber);

            // DonationRecord Configuration
            modelBuilder.Entity<DonationRecord>()
                .HasOne(dr => dr.Donor)
                .WithMany(d => d.DonationRecords)
                .HasForeignKey(dr => dr.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DonationRecord>()
                .HasIndex(dr => dr.DonorId);

            modelBuilder.Entity<DonationRecord>()
                .HasIndex(dr => dr.DonationDate);

            // BloodInventory Configuration
            modelBuilder.Entity<BloodInventory>()
                .HasIndex(bi => bi.BloodGroup);

            modelBuilder.Entity<BloodInventory>()
                .HasIndex(bi => bi.ExpiryDate);
        }
    }
}