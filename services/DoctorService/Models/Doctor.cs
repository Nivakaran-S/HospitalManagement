namespace DoctorService.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string KeycloakUserId { get; set; }    
        public string Name { get; set; }
        public string Speciality { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Qualifications { get; set; }
        public int ExperienceYears { get; set; }
        public string LicenseNumber { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
    }

    public class DoctorAvailability
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30; // Default 30 minutes per patient
        public bool IsAvailable { get; set; } = true;
        
        public Doctor Doctor { get; set; }
    }

    public class DoctorLeave
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        public Doctor Doctor { get; set; }
    }
}