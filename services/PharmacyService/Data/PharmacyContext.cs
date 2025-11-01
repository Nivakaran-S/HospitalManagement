using Microsoft.EntityFrameworkCore;
using PharmacyService.Models;

namespace PharmacyService.Data
{
    public class PharmacyContext : DbContext
    {
        public PharmacyContext(DbContextOptions<PharmacyContext> options)
            : base(options) { }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineStock> MedicineStocks { get; set; }
        public DbSet<MedicineSale> MedicineSales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Medicine Configuration
            modelBuilder.Entity<Medicine>()
                .HasIndex(m => m.Name);

            modelBuilder.Entity<Medicine>()
                .HasIndex(m => m.Category);

            modelBuilder.Entity<Medicine>()
                .HasIndex(m => m.BatchNumber);

            // MedicineStock Configuration
            modelBuilder.Entity<MedicineStock>()
                .HasOne(ms => ms.Medicine)
                .WithMany(m => m.StockHistory)
                .HasForeignKey(ms => ms.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MedicineStock>()
                .HasIndex(ms => ms.MedicineId);

            modelBuilder.Entity<MedicineStock>()
                .HasIndex(ms => ms.TransactionDate);

            // MedicineSale Configuration
            modelBuilder.Entity<MedicineSale>()
                .HasOne(ms => ms.Medicine)
                .WithMany(m => m.Sales)
                .HasForeignKey(ms => ms.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MedicineSale>()
                .HasIndex(ms => ms.MedicineId);

            modelBuilder.Entity<MedicineSale>()
                .HasIndex(ms => ms.PatientId);

            modelBuilder.Entity<MedicineSale>()
                .HasIndex(ms => ms.SaleDate);
        }
    }
}