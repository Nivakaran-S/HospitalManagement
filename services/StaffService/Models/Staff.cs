namespace StaffService.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string KeycloakUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StaffRoleId { get; set; } // Foreign key to StaffRole
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public string EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public string Address { get; set; }
        public string Qualification { get; set; }
        public string ShiftTiming { get; set; } // Morning, Evening, Night
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public StaffRole StaffRole { get; set; }
        public ICollection<StaffAttendance> Attendances { get; set; } = new List<StaffAttendance>();
        public ICollection<StaffLeave> Leaves { get; set; } = new List<StaffLeave>();
    }

    public class StaffRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; } // Pharmacist, Receptionist, Nurse, Lab Technician, etc.
        public string Description { get; set; }
        public string KeycloakRoleName { get; set; } // Maps to Keycloak role
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }

        // Navigation Properties
        public ICollection<StaffRolePermission> Permissions { get; set; } = new List<StaffRolePermission>();
        public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    }

    public class StaffRolePermission
    {
        public int Id { get; set; }
        public int StaffRoleId { get; set; }
        public string Module { get; set; } // Patients, Appointments, Billing, Pharmacy, Lab, Inventory, etc.
        public bool CanView { get; set; } = false;
        public bool CanCreate { get; set; } = false;
        public bool CanUpdate { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public string AdditionalPermissions { get; set; } // JSON for specific permissions
        
        public StaffRole StaffRole { get; set; }
    }

    public class StaffAttendance
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; } = "Present"; // Present, Absent, Late, HalfDay
        public string Notes { get; set; }
        
        public Staff Staff { get; set; }
    }

    public class StaffLeave
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } // Sick, Casual, Annual
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        
        public Staff Staff { get; set; }
    }
}