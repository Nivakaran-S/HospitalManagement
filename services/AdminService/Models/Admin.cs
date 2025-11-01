namespace AdminService.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string KeycloakUserId { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Department { get; set; }
        public string Privileges { get; set; } // JSON string of privileges
        public bool IsSuperAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
    }

    public class SystemLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserRole { get; set; }
        public string Action { get; set; }
        public string Entity { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SystemSettings
    {
        public int Id { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
    }

    public class UserManagementRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // admin, doctor, patient, staff
        public bool Enabled { get; set; } = true;
    }
}