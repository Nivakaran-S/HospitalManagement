using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffService.Data;
using StaffService.Models;
using System.Security.Claims;

namespace StaffService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly StaffContext _context;
        private readonly ILogger<StaffController> _logger;

        public StaffController(StaffContext context, ILogger<StaffController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffMembers(
            [FromQuery] int? roleId,
            [FromQuery] string? department,
            [FromQuery] bool activeOnly = true)
        {
            var query = _context.StaffMembers
                .Include(s => s.StaffRole)
                .Include(s => s.Attendances)
                .Include(s => s.Leaves)
                .AsQueryable();

            if (activeOnly)
                query = query.Where(s => s.IsActive);

            if (roleId.HasValue)
                query = query.Where(s => s.StaffRoleId == roleId.Value);

            if (!string.IsNullOrEmpty(department))
                query = query.Where(s => s.Department == department);

            var staff = await query
                .OrderBy(s => s.LastName)
                .ToListAsync();

            return Ok(staff);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("me")]
        public async Task<ActionResult<Staff>> GetMyProfile()
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var staff = await _context.StaffMembers
                .Include(s => s.Attendances)
                .Include(s => s.Leaves)
                .FirstOrDefaultAsync(s => s.KeycloakUserId == keycloakUserId);
            
            if (staff == null) return NotFound();
            return staff;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaff(int id)
        {
            var staff = await _context.StaffMembers
                .Include(s => s.Attendances)
                .Include(s => s.Leaves)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (staff == null) return NotFound();
            return staff;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Staff>> PostStaff(Staff staff)
        {
            if (await _context.StaffMembers.AnyAsync(s => s.Email == staff.Email))
                return BadRequest("Email already exists");

            if (await _context.StaffMembers.AnyAsync(s => s.EmployeeId == staff.EmployeeId))
                return BadRequest("Employee ID already exists");

            staff.CreatedAt = DateTime.UtcNow;
            staff.UpdatedAt = DateTime.UtcNow;
            
            _context.StaffMembers.Add(staff);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetStaff), new { id = staff.Id }, staff);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, Staff staff)
        {
            if (id != staff.Id) return BadRequest();
            
            var dbStaff = await _context.StaffMembers.FindAsync(id);
            if (dbStaff == null) return NotFound();

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "staff" && dbStaff.KeycloakUserId != keycloakUserId)
                return Forbid();

            dbStaff.FirstName = staff.FirstName;
            dbStaff.LastName = staff.LastName;
            dbStaff.ContactNumber = staff.ContactNumber;
            dbStaff.Email = staff.Email;
            dbStaff.Address = staff.Address;
            dbStaff.UpdatedAt = DateTime.UtcNow;

            if (userRole == "admin")
            {
                dbStaff.StaffRoleId = staff.StaffRoleId;
                dbStaff.Department = staff.Department;
                dbStaff.Salary = staff.Salary;
                dbStaff.ShiftTiming = staff.ShiftTiming;
                dbStaff.Qualification = staff.Qualification;
                dbStaff.IsActive = staff.IsActive;
            }

            _context.Entry(dbStaff).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null) return NotFound();
            
            staff.IsActive = false;
            staff.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{id}/mark-attendance")]
        public async Task<ActionResult<StaffAttendance>> MarkAttendance(int id, StaffAttendance attendance)
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null) return NotFound("Staff not found");

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "staff" && staff.KeycloakUserId != keycloakUserId)
                return Forbid();

            var existingAttendance = await _context.StaffAttendances
                .FirstOrDefaultAsync(sa => sa.StaffId == id && sa.Date.Date == attendance.Date.Date);

            if (existingAttendance != null)
                return BadRequest("Attendance already marked for this date");

            attendance.StaffId = id;
            _context.StaffAttendances.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStaff), new { id = staff.Id }, attendance);
        }

        [Authorize(Roles = "staff,admin")]
        [HttpPost("{id}/request-leave")]
        public async Task<ActionResult<StaffLeave>> RequestLeave(int id, StaffLeave leave)
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null) return NotFound("Staff not found");

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (staff.KeycloakUserId != keycloakUserId)
                return Forbid();

            leave.StaffId = id;
            leave.RequestedDate = DateTime.UtcNow;
            leave.Status = "Pending";

            _context.StaffLeaves.Add(leave);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStaff), new { id = staff.Id }, leave);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("leave/{leaveId}/approve")]
        public async Task<IActionResult> ApproveLeave(int leaveId)
        {
            var leave = await _context.StaffLeaves.FindAsync(leaveId);
            if (leave == null) return NotFound();

            var approverName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";

            leave.Status = "Approved";
            leave.ApprovedBy = approverName;
            leave.ApprovedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("leave/{leaveId}/reject")]
        public async Task<IActionResult> RejectLeave(int leaveId)
        {
            var leave = await _context.StaffLeaves.FindAsync(leaveId);
            if (leave == null) return NotFound();

            var approverName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";

            leave.Status = "Rejected";
            leave.ApprovedBy = approverName;
            leave.ApprovedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("attendance/report")]
        public async Task<ActionResult<object>> GetAttendanceReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.StaffAttendances.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(sa => sa.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(sa => sa.Date <= toDate.Value);

            var report = await query
                .GroupBy(sa => sa.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(report);
        }
    }
}