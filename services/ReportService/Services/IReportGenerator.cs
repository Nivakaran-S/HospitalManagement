using ReportService.Models;

namespace ReportService.Services
{
    public interface IReportGenerator
    {
        Task<FinancialSummaryReport> GenerateFinancialSummary(DateTime startDate, DateTime endDate);
        Task<PatientStatisticsReport> GeneratePatientStatistics(DateTime startDate, DateTime endDate);
        Task<AppointmentStatisticsReport> GenerateAppointmentStatistics(DateTime startDate, DateTime endDate);
        Task<InventoryReport> GenerateInventoryReport();
        Task<PharmacyReport> GeneratePharmacyReport(DateTime startDate, DateTime endDate);
        Task<LabStatisticsReport> GenerateLabStatistics(DateTime startDate, DateTime endDate);
        Task<StaffReport> GenerateStaffReport();
    }
}