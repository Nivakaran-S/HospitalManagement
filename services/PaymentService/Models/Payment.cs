namespace PaymentService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int? BillingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; }  // CreditCard, DebitCard, Cash, Insurance, OnlineTransfer, Cheque
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed, Refunded
        public string TransactionId { get; set; }
        public string PaymentGateway { get; set; } // Stripe, PayPal, etc.
        public string CardLast4Digits { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public string FailureReason { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<PaymentRefund> Refunds { get; set; } = new List<PaymentRefund>();
    }

    public class PaymentRefund
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateTime RefundDate { get; set; } = DateTime.UtcNow;
        public string RefundedBy { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
        public string TransactionId { get; set; }
        
        public Payment Payment { get; set; }
    }

    public class PaymentGatewayConfig
    {
        public int Id { get; set; }
        public string GatewayName { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string WebhookUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public string SupportedMethods { get; set; } // JSON array
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class InsuranceClaim
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int? BillingId { get; set; }
        public string InsuranceProvider { get; set; }
        public string PolicyNumber { get; set; }
        public string ClaimNumber { get; set; }
        public decimal ClaimAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public string Status { get; set; } = "Submitted"; // Submitted, UnderReview, Approved, Rejected, Paid
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public string Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}