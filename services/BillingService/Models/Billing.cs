namespace BillingService.Models
{
    public class Billing
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int? AppointmentId { get; set; }
        public int? MedicalRecordId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BillingDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Pending, PartiallyPaid
        public string Description { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal DueAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<BillingItem> BillingItems { get; set; } = new List<BillingItem>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }

    public class BillingItem
    {
        public int Id { get; set; }
        public int BillingId { get; set; }
        public string ItemType { get; set; } // Consultation, Lab, Medicine, Procedure, Room
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Billing Billing { get; set; }
    }

    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int BillingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // Cash, Card, Insurance, Online
        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string ReceivedBy { get; set; }
        public string Notes { get; set; }
        
        public Billing Billing { get; set; }
    }
}