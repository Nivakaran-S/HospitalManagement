namespace AppointmentService.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, NoShow
        public string Reason { get; set; }
        public string Notes { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancelledBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}