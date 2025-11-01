namespace PharmacyService.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GenericName { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // Antibiotic, Painkiller, etc.
        public string DosageForm { get; set; } // Tablet, Capsule, Syrup, Injection
        public string Strength { get; set; } // e.g., "500mg", "10ml"
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; } = 50;
        public decimal UnitPrice { get; set; }
        public string ExpiryDate { get; set; }
        public string BatchNumber { get; set; }
        public string StorageConditions { get; set; }
        public bool RequiresPrescription { get; set; } = true;
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<MedicineStock> StockHistory { get; set; } = new List<MedicineStock>();
        public ICollection<MedicineSale> Sales { get; set; } = new List<MedicineSale>();
    }

    public class MedicineStock
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string TransactionType { get; set; } // Purchase, Sale, Return, Adjustment
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Supplier { get; set; }
        public string Notes { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string RecordedBy { get; set; }
        
        public Medicine Medicine { get; set; }
    }

    public class MedicineSale
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public int? PatientId { get; set; }
        public int? PrescriptionId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public string SoldBy { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        
        public Medicine Medicine { get; set; }
    }
}