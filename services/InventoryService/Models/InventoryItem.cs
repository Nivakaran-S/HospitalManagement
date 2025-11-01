namespace InventoryService.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Category { get; set; } // Medical Equipment, Surgical Supplies, Office Supplies, etc.
        public int Quantity { get; set; }
        public string Unit { get; set; } // pcs, boxes, liters, kg
        public decimal UnitPrice { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int ReorderLevel { get; set; } = 10;
        public string Supplier { get; set; }
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string TransactionType { get; set; } // Purchase, Issue, Return, Adjustment
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public string Notes { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public InventoryItem InventoryItem { get; set; }
    }

    public class InventoryRequest
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; }
        public int RequestedQuantity { get; set; }
        public string RequestedBy { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Fulfilled
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        
        public InventoryItem InventoryItem { get; set; }
    }
}