namespace PrescriptionService.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int? MedicalRecordId { get; set; }
        public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, InPharmacy, PartiallyDispensed, Dispensed, Cancelled
        public DateTime? SentToPharmacyAt { get; set; }
        public DateTime? ValidUntil { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<PrescriptionMedicine> Medicines { get; set; } = new List<PrescriptionMedicine>();
    }

    public class PrescriptionMedicine
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; } // e.g., "3 times a day", "Every 8 hours"
        public string Duration { get; set; } // e.g., "7 days", "2 weeks"
        public string Route { get; set; } // Oral, Injection, Topical, etc.
        public int Quantity { get; set; }
        public string Instructions { get; set; }
        public bool IsDispensed { get; set; } = false;
        public DateTime? DispensedDate { get; set; }
        public string DispensedBy { get; set; }
        public string PharmacyNotes { get; set; }
        
        public Prescription Prescription { get; set; }
    }
}