namespace LabService.Models
{
    public class LabTest
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string TestName { get; set; }
        public string TestCategory { get; set; } // Blood, Urine, Imaging, Biopsy, etc.
        public DateTime OrderedDate { get; set; } = DateTime.UtcNow;
        public DateTime? TestDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Result { get; set; }
        public string ReferenceRange { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
        public string Notes { get; set; }
        public string TechnicianName { get; set; }
        public decimal Cost { get; set; }
        public bool IsUrgent { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<LabTestResult> TestResults { get; set; } = new List<LabTestResult>();
    }

    public class LabTestResult
    {
        public int Id { get; set; }
        public int LabTestId { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string ReferenceRange { get; set; }
        public bool IsAbnormal { get; set; } = false;
        public string Notes { get; set; }
        
        public LabTest LabTest { get; set; }
    }

    public class LabTestTemplate
    {
        public int Id { get; set; }
        public string TestName { get; set; }
        public string TestCategory { get; set; }
        public string Description { get; set; }
        public decimal StandardCost { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public string SampleType { get; set; } // Blood, Urine, Tissue, etc.
        public string PreparationInstructions { get; set; }
        public bool IsActive { get; set; } = true;
    }
}