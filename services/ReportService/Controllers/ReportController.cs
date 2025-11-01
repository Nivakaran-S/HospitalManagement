using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;
using ReportService.Services;
using System.Security.Claims;

namespace ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportContext _context;
        private readonly IReportGenerator _reportGenerator;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            ReportContext context,
            IReportGenerator reportGenerator,
            ILogger<ReportController> logger)
        {
            _context = context;
            _reportGenerator = reportGenerator;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports(
            [FromQuery] string? reportType,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Reports.AsQueryable();

            if (!string.IsNullOrEmpty(reportType))
                query = query.Where(r => r.ReportType == reportType);

            if (fromDate.HasValue)
                query = query.Where(r => r.GeneratedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.GeneratedDate <= toDate.Value);

            var reports = await query
                .OrderByDescending(r => r.GeneratedDate)
                .ToListAsync();

            return Ok(reports);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();
            return report;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("financial-summary")]
        public async Task<ActionResult<FinancialSummaryReport>> GenerateFinancialSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var report = await _reportGenerator.GenerateFinancialSummary(
                startDate ?? DateTime.Today.AddMonths(-1),
                endDate ?? DateTime.Today);

            await SaveReport("Financial", "Financial Summary", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("patient-statistics")]
        public async Task<ActionResult<PatientStatisticsReport>> GeneratePatientStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var report = await _reportGenerator.GeneratePatientStatistics(
                startDate ?? DateTime.Today.AddMonths(-1),
                endDate ?? DateTime.Today);

            await SaveReport("Clinical", "Patient Statistics", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("appointment-statistics")]
        public async Task<ActionResult<AppointmentStatisticsReport>> GenerateAppointmentStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var report = await _reportGenerator.GenerateAppointmentStatistics(
                startDate ?? DateTime.Today.AddMonths(-1),
                endDate ?? DateTime.Today);

            await SaveReport("Operational", "Appointment Statistics", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("inventory-report")]
        public async Task<ActionResult<InventoryReport>> GenerateInventoryReport()
        {
            var report = await _reportGenerator.GenerateInventoryReport();

            await SaveReport("Inventory", "Inventory Report", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("pharmacy-report")]
        public async Task<ActionResult<PharmacyReport>> GeneratePharmacyReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var report = await _reportGenerator.GeneratePharmacyReport(
                startDate ?? DateTime.Today.AddMonths(-1),
                endDate ?? DateTime.Today);

            await SaveReport("Pharmacy", "Pharmacy Report", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("lab-statistics")]
        public async Task<ActionResult<LabStatisticsReport>> GenerateLabStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var report = await _reportGenerator.GenerateLabStatistics(
                startDate ?? DateTime.Today.AddMonths(-1),
                endDate ?? DateTime.Today);

            await SaveReport("Lab", "Lab Statistics", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("staff-report")]
        public async Task<ActionResult<StaffReport>> GenerateStaffReport()
        {
            var report = await _reportGenerator.GenerateStaffReport();

            await SaveReport("HR", "Staff Report", report);

            return Ok(report);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("comprehensive")]
        public async Task<ActionResult<object>> GenerateComprehensiveReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            var comprehensive = new
            {
                GeneratedDate = DateTime.UtcNow,
                Period = new { Start = start, End = end },
                Financial = await _reportGenerator.GenerateFinancialSummary(start, end),
                Patients = await _reportGenerator.GeneratePatientStatistics(start, end),
                Appointments = await _reportGenerator.GenerateAppointmentStatistics(start, end),
                Pharmacy = await _reportGenerator.GeneratePharmacyReport(start, end),
                Lab = await _reportGenerator.GenerateLabStatistics(start, end),
                Inventory = await _reportGenerator.GenerateInventoryReport(),
                Staff = await _reportGenerator.GenerateStaffReport()
            };

            await SaveReport("Comprehensive", "Comprehensive Hospital Report", comprehensive);

            return Ok(comprehensive);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<ReportTemplate>>> GetTemplates()
        {
            var templates = await _context.ReportTemplates
                .Where(rt => rt.IsActive)
                .OrderBy(rt => rt.TemplateName)
                .ToListAsync();

            return Ok(templates);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("templates")]
        public async Task<ActionResult<ReportTemplate>> CreateTemplate(ReportTemplate template)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
            template.CreatedBy = userName;
            template.CreatedAt = DateTime.UtcNow;

            _context.ReportTemplates.Add(template);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplates), new { id = template.Id }, template);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task SaveReport(string reportType, string reportName, object reportData)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var report = new Report
            {
                ReportType = reportType,
                ReportName = reportName,
                GeneratedBy = userName,
                GeneratedDate = DateTime.UtcNow,
                ReportData = System.Text.Json.JsonSerializer.Serialize(reportData),
                Status = "Completed",
                Format = "JSON"
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
        }
    }
}