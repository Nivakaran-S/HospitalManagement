namespace ReportService.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string ReportType { get; set; } // Financial, Operational, Clinical, Inventory, etc.
        public string ReportName { get; set; }
        public string Description { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public string GeneratedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Parameters { get; set; } // JSON string of parameters
        public string ReportData { get; set; } // JSON string of report data
        public string Status { get; set; } = "Completed"; // Pending, Processing, Completed, Failed
        public string Format { get; set; } = "JSON"; // JSON, PDF, Excel, CSV
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ReportTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string ReportType { get; set; }
        public string Description { get; set; }
        public string QueryTemplate { get; set; } // SQL or data fetch logic
        public string ParameterSchema { get; set; } // JSON schema for parameters
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ScheduledReport
    {
        public int Id { get; set; }
        public int ReportTemplateId { get; set; }
        public string ReportName { get; set; }
        public string Schedule { get; set; } // Cron expression or frequency
        public string Recipients { get; set; } // Comma-separated emails
        public string Parameters { get; set; } // JSON parameters
        public bool IsActive { get; set; } = true;
        public DateTime? LastRunDate { get; set; }
        public DateTime? NextRunDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ReportTemplate ReportTemplate { get; set; }
    }

    // DTOs for specific reports
    public class FinancialSummaryReport
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public decimal OutstandingPayments { get; set; }
        public decimal CollectedPayments { get; set; }
        public int TotalBills { get; set; }
        public int PaidBills { get; set; }
        public int PendingBills { get; set; }
    }

    public class PatientStatisticsReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalPatients { get; set; }
        public int NewPatients { get; set; }
        public int ActivePatients { get; set; }
        public Dictionary<string, int> PatientsByDepartment { get; set; }
        public Dictionary<string, int> PatientsByAge { get; set; }
        public Dictionary<string, int> PatientsByGender { get; set; }
    }

    public class AppointmentStatisticsReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public Dictionary<string, int> AppointmentsByDepartment { get; set; }
        public Dictionary<string, int> AppointmentsByDoctor { get; set; }
        public double AverageWaitTime { get; set; }
    }

    public class InventoryReport
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public List<InventoryItemSummary> ItemSummaries { get; set; }
    }

    public class InventoryItemSummary
    {
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public string Status { get; set; }
    }

    public class PharmacyReport
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalPrescriptions { get; set; }
        public int DispensedPrescriptions { get; set; }
        public int PendingPrescriptions { get; set; }
        public List<TopSellingMedicine> TopMedicines { get; set; }
        public decimal InventoryValue { get; set; }
        public int LowStockCount { get; set; }
    }

    public class TopSellingMedicine
    {
        public string MedicineName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class LabStatisticsReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalTests { get; set; }
        public int CompletedTests { get; set; }
        public int PendingTests { get; set; }
        public int UrgentTests { get; set; }
        public Dictionary<string, int> TestsByCategory { get; set; }
        public double AverageCompletionTime { get; set; }
    }

    public class StaffReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalStaff { get; set; }
        public int ActiveStaff { get; set; }
        public Dictionary<string, int> StaffByRole { get; set; }
        public Dictionary<string, int> StaffByDepartment { get; set; }
        public double AverageAttendanceRate { get; set; }
        public int PendingLeaveRequests { get; set; }
    }
}