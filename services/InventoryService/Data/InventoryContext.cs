using Microsoft.EntityFrameworkCore;
using InventoryService.Models;

namespace InventoryService.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options)
            : base(options) { }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<InventoryRequest> InventoryRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // InventoryItem Configuration
            modelBuilder.Entity<InventoryItem>()
                .HasIndex(ii => ii.ItemCode)
                .IsUnique();

            modelBuilder.Entity<InventoryItem>()
                .HasIndex(ii => ii.ItemName);

            modelBuilder.Entity<InventoryItem>()
                .HasIndex(ii => ii.Category);

            // InventoryTransaction Configuration
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.InventoryItem)
                .WithMany(ii => ii.Transactions)
                .HasForeignKey(it => it.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => it.InventoryItemId);

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => it.TransactionDate);

            // InventoryRequest Configuration
            modelBuilder.Entity<InventoryRequest>()
                .HasOne(ir => ir.InventoryItem)
                .WithMany()
                .HasForeignKey(ir => ir.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryRequest>()
                .HasIndex(ir => ir.Status);

            modelBuilder.Entity<InventoryRequest>()
                .HasIndex(ir => ir.RequestDate);
        }
    }
}