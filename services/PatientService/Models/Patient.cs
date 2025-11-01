namespace PatientService.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string KeycloakUserId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string InsuranceProvider { get; set; }
        public string InsuranceNumber { get; set; }
        
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public string ChronicConditions { get; set; }
        public string CurrentMedications { get; set; }
        public string Immunizations { get; set; }
        public double Height { get; set; } 
        public double Weight { get; set; } 
        public string BloodPressure { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
    }

    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime VisitDate { get; set; }
        public string ChiefComplaint { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public string PrescribedMedications { get; set; }
        public string FollowUpInstructions { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public decimal ConsultationFee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Patient Patient { get; set; }
    }

    public class VitalSign
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public string RecordedBy { get; set; } 
        public double Temperature { get; set; } 
        public string BloodPressure { get; set; } 
        public int HeartRate { get; set; }
        public int RespiratoryRate { get; set; }
        public double OxygenSaturation { get; set; } 
        public double Height { get; set; } 
        public double Weight { get; set; } 
        public string Notes { get; set; }
        
        public Patient Patient { get; set; }
    }
}