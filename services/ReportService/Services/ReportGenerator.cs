using ReportService.Models;
using System.Net.Http;
using System.Text.Json;

namespace ReportService.Services
{
    public class ReportGenerator : IReportGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReportGenerator> _logger;
        private readonly IConfiguration _configuration;

        public ReportGenerator(
            HttpClient httpClient,
            ILogger<ReportGenerator> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<FinancialSummaryReport> GenerateFinancialSummary(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Call Billing Service API
                var billingUrl = _configuration["Services:BillingService"] ?? "http://billingservice";
                var response = await _httpClient.GetAsync(
                    $"{billingUrl}/api/Billing/report/summary?fromDate={startDate:yyyy-MM-dd}&toDate={endDate:yyyy-MM-dd}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var billingData = JsonSerializer.Deserialize<dynamic>(content);

                    return new FinancialSummaryReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalRevenue = billingData?.GetProperty("totalBilled").GetDecimal() ?? 0,
                        CollectedPayments = billingData?.GetProperty("totalPaid").GetDecimal() ?? 0,
                        OutstandingPayments = billingData?.GetProperty("totalDue").GetDecimal() ?? 0,
                        TotalBills = billingData?.GetProperty("paidCount").GetInt32() ?? 0 +
                                    billingData?.GetProperty("unpaidCount").GetInt32() ?? 0 +
                                    billingData?.GetProperty("partiallyPaidCount").GetInt32() ?? 0,
                        PaidBills = billingData?.GetProperty("paidCount").GetInt32() ?? 0,
                        PendingBills = billingData?.GetProperty("unpaidCount").GetInt32() ?? 0 +
                                      billingData?.GetProperty("partiallyPaidCount").GetInt32() ?? 0,
                        NetIncome = (billingData?.GetProperty("totalPaid").GetDecimal() ?? 0) * 0.7m, // Example calculation
                        TotalExpenses = (billingData?.GetProperty("totalPaid").GetDecimal() ?? 0) * 0.3m
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating financial summary");
            }

            return new FinancialSummaryReport { ReportDate = DateTime.UtcNow };
        }

        public async Task<PatientStatisticsReport> GeneratePatientStatistics(DateTime startDate, DateTime endDate)
        {
            try
            {
                var patientUrl = _configuration["Services:PatientService"] ?? "http://patientservice";
                var response = await _httpClient.GetAsync($"{patientUrl}/api/Patient");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Parse and aggregate patient data
                    return new PatientStatisticsReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalPatients = 0, // Implement actual logic
                        NewPatients = 0,
                        ActivePatients = 0,
                        PatientsByDepartment = new Dictionary<string, int>(),
                        PatientsByAge = new Dictionary<string, int>(),
                        PatientsByGender = new Dictionary<string, int>()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating patient statistics");
            }

            return new PatientStatisticsReport { ReportDate = DateTime.UtcNow };
        }

        public async Task<AppointmentStatisticsReport> GenerateAppointmentStatistics(DateTime startDate, DateTime endDate)
        {
            try
            {
                var appointmentUrl = _configuration["Services:AppointmentService"] ?? "http://appointmentservice";
                var response = await _httpClient.GetAsync(
                    $"{appointmentUrl}/api/Appointment?fromDate={startDate:yyyy-MM-dd}&toDate={endDate:yyyy-MM-dd}");

                if (response.IsSuccessStatusCode)
                {
                    // Parse appointment data
                    return new AppointmentStatisticsReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalAppointments = 0,
                        CompletedAppointments = 0,
                        CancelledAppointments = 0,
                        NoShowAppointments = 0,
                        AppointmentsByDepartment = new Dictionary<string, int>(),
                        AppointmentsByDoctor = new Dictionary<string, int>(),
                        AverageWaitTime = 0
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating appointment statistics");
            }

            return new AppointmentStatisticsReport { ReportDate = DateTime.UtcNow };
        }

        public async Task<InventoryReport> GenerateInventoryReport()
        {
            try
            {
                var inventoryUrl = _configuration["Services:InventoryService"] ?? "http://inventoryservice";
                var response = await _httpClient.GetAsync($"{inventoryUrl}/api/Inventory/report/value");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var inventoryData = JsonSerializer.Deserialize<JsonElement>(content);

                    return new InventoryReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalInventoryValue = inventoryData.GetProperty("totalValue").GetDecimal(),
                        TotalItems = inventoryData.GetProperty("totalItems").GetInt32(),
                        LowStockItems = inventoryData.GetProperty("lowStockCount").GetInt32(),
                        OutOfStockItems = inventoryData.GetProperty("outOfStockCount").GetInt32(),
                        ItemSummaries = new List<InventoryItemSummary>()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating inventory report");
            }

            return new InventoryReport { ReportDate = DateTime.UtcNow };
        }

        public async Task<PharmacyReport> GeneratePharmacyReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var pharmacyUrl = _configuration["Services:PharmacyService"] ?? "http://pharmacyservice";
                var salesResponse = await _httpClient.GetAsync(
                    $"{pharmacyUrl}/api/Pharmacy/sales-report?fromDate={startDate:yyyy-MM-dd}&toDate={endDate:yyyy-MM-dd}");

                if (salesResponse.IsSuccessStatusCode)
                {
                    var content = await salesResponse.Content.ReadAsStringAsync();
                    var salesData = JsonSerializer.Deserialize<JsonElement>(content);

                    return new PharmacyReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalSales = salesData.GetProperty("totalSales").GetDecimal(),
                        TotalPrescriptions = 0, // Get from prescription service
                        DispensedPrescriptions = 0,
                        PendingPrescriptions = 0,
                        TopMedicines = new List<TopSellingMedicine>(),
                        InventoryValue = 0,
                        LowStockCount = 0
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating pharmacy report");
            }

            return new PharmacyReport { ReportDate = DateTime.UtcNow };
        }

        public async Task<LabStatisticsReport> GenerateLabStatistics(DateTime startDate, DateTime endDate)
        {
            try
            {
                var labUrl = _configuration["Services:LabService"] ?? "http://labservice";
                var response = await _httpClient.GetAsync(
                    $"{labUrl}/api/LabTest/report/summary?fromDate={startDate:yyyy-MM-dd}&toDate={endDate:yyyy-MM-dd}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var labData = JsonSerializer.Deserialize<JsonElement>(content);

                    return new LabStatisticsReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalTests = labData.GetProperty("totalTests").GetInt32(),
                        CompletedTests = labData.GetProperty("completedTests").GetInt32(),
                        PendingTests = labData.GetProperty("pendingTests").GetInt32(),
                        UrgentTests = labData.GetProperty("urgentTests").GetInt32(),
                        TestsByCategory = new Dictionary<string, int>(),
                        AverageCompletionTime = 0
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating lab statistics");
            }

            return new LabStatisticsReport { ReportDate = DateTime.UtcNow };
        }

        public async Task<StaffReport> GenerateStaffReport()
        {
            try
            {
                var staffUrl = _configuration["Services:StaffService"] ?? "http://staffservice";
                var response = await _httpClient.GetAsync($"{staffUrl}/api/Staff");

                if (response.IsSuccessStatusCode)
                {
                    // Parse staff data
                    return new StaffReport
                    {
                        ReportDate = DateTime.UtcNow,
                        TotalStaff = 0,
                        ActiveStaff = 0,
                        StaffByRole = new Dictionary<string, int>(),
                        StaffByDepartment = new Dictionary<string, int>(),
                        AverageAttendanceRate = 0,
                        PendingLeaveRequests = 0
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating staff report");
            }

            return new StaffReport { ReportDate = DateTime.UtcNow };
        }
    }
}