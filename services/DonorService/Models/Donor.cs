namespace DonorService.Models
{
    public class Donor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BloodGroup { get; set; } // A+, A-, B+, B-, AB+, AB-, O+, O-
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime LastDonationDate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string MedicalHistory { get; set; }
        public double Weight { get; set; } // in kg
        public string EmergencyContact { get; set; }
        public string EmergencyContactNumber { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<DonationRecord> DonationRecords { get; set; } = new List<DonationRecord>();
    }

    public class DonationRecord
    {
        public int Id { get; set; }
        public int DonorId { get; set; }
        public DateTime DonationDate { get; set; }
        public string BloodGroup { get; set; }
        public int UnitsCollected { get; set; } // in mL
        public string CollectedBy { get; set; } // Staff name
        public string Notes { get; set; }
        public bool IsSafe { get; set; } = true; // After screening
        public string ScreeningNotes { get; set; }
        
        public Donor Donor { get; set; }
    }

    public class BloodInventory
    {
        public int Id { get; set; }
        public string BloodGroup { get; set; }
        public int UnitsAvailable { get; set; } // in mL
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string Location { get; set; } // Blood bank location
        public DateTime ExpiryDate { get; set; }
    }
}