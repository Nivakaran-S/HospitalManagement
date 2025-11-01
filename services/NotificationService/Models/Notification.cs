namespace NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public string RecipientType { get; set; } // Patient, Doctor, Staff, Admin
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string NotificationType { get; set; } // Appointment, Prescription, Lab, Billing, General
        public string Channel { get; set; } // Email, SMS, InApp, Push
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
        public DateTime SentAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Sent, Failed, Delivered
        public string FailureReason { get; set; }
        public int RetryCount { get; set; } = 0;
        public string ReferenceId { get; set; } // ID of related entity (appointment, prescription, etc.)
        public string ReferenceType { get; set; } // Appointment, Prescription, etc.
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class NotificationTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string NotificationType { get; set; }
        public string Channel { get; set; }
        public string Subject { get; set; }
        public string MessageTemplate { get; set; } // With placeholders like {{patientName}}
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
    }

    public class NotificationPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserType { get; set; } // Patient, Doctor, Staff
        public bool EmailEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;
        public bool InAppEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = true;
        
        // Notification type preferences
        public bool AppointmentReminders { get; set; } = true;
        public bool PrescriptionUpdates { get; set; } = true;
        public bool LabResults { get; set; } = true;
        public bool BillingAlerts { get; set; } = true;
        public bool GeneralAnnouncements { get; set; } = true;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class NotificationLog
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string Action { get; set; } // Created, Sent, Failed, Retry, Delivered, Read
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public Notification Notification { get; set; }
    }

    // DTOs
    public class SendNotificationRequest
    {
        public int RecipientId { get; set; }
        public string RecipientType { get; set; }
        public string NotificationType { get; set; }
        public string Channel { get; set; } = "Email";
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; } = "Normal";
        public DateTime? ScheduledFor { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceType { get; set; }
    }

    public class BulkNotificationRequest
    {
        public List<int> RecipientIds { get; set; }
        public string RecipientType { get; set; }
        public string NotificationType { get; set; }
        public string Channel { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}