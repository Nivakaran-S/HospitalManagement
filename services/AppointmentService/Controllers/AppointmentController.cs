using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  
using AppointmentService.Data; 
using AppointmentService.Models;
using System.Security.Claims;

namespace AppointmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentContext _context;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(AppointmentContext context, ILogger<AppointmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(
            [FromQuery] int? doctorId,
            [FromQuery] int? patientId,
            [FromQuery] DateTime? date,
            [FromQuery] string? status)
        {
            var query = _context.Appointments.AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return Ok(appointments);
        }

        [Authorize(Roles = "admin,patient,doctor,staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            // Patients can only see their own appointments
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "patient")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // Note: You'd need to validate patientId matches keycloak user
            }

            return appointment;
        }

        [Authorize(Roles = "patient,admin,staff")]
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            // Check for double booking
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => 
                    a.DoctorId == appointment.DoctorId &&
                    a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                    a.AppointmentTime == appointment.AppointmentTime &&
                    a.Status != "Cancelled");

            if (existingAppointment != null)
                return BadRequest("This time slot is already booked");

            // Check if appointment is within doctor's availability
            // This would need a call to DoctorService API to validate

            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;
            appointment.Status = "Scheduled";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id) return BadRequest();

            var dbAppointment = await _context.Appointments.FindAsync(id);
            if (dbAppointment == null) return NotFound();

            // Check for double booking if time is changed
            if (dbAppointment.AppointmentDate != appointment.AppointmentDate || 
                dbAppointment.AppointmentTime != appointment.AppointmentTime)
            {
                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => 
                        a.Id != id &&
                        a.DoctorId == appointment.DoctorId &&
                        a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                        a.AppointmentTime == appointment.AppointmentTime &&
                        a.Status != "Cancelled");

                if (existingAppointment != null)
                    return BadRequest("This time slot is already booked");
            }

            dbAppointment.AppointmentDate = appointment.AppointmentDate;
            dbAppointment.AppointmentTime = appointment.AppointmentTime;
            dbAppointment.DurationMinutes = appointment.DurationMinutes;
            dbAppointment.Status = appointment.Status;
            dbAppointment.Reason = appointment.Reason;
            dbAppointment.Notes = appointment.Notes;
            dbAppointment.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbAppointment).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Appointments.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }
            
            return NoContent();
        }

        [Authorize(Roles = "admin,patient,staff,doctor")]
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] string reason)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            if (appointment.Status == "Cancelled")
                return BadRequest("Appointment is already cancelled");

            if (appointment.Status == "Completed")
                return BadRequest("Cannot cancel a completed appointment");

            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            
            appointment.Status = "Cancelled";
            appointment.CancellationReason = reason;
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancelledBy = userName;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [Authorize(Roles = "doctor,staff")]
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            if (appointment.Status == "Cancelled")
                return BadRequest("Cannot complete a cancelled appointment");

            appointment.Status = "Completed";
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("doctor/{doctorId}/today")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetTodayAppointments(int doctorId)
        {
            var today = DateTime.Today;
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && 
                           a.AppointmentDate.Date == today &&
                           a.Status == "Scheduled")
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            return Ok(appointments);
        }

        [Authorize(Roles = "patient")]
        [HttpGet("my-appointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetMyAppointments([FromQuery] int patientId)
        {
            // Note: Should validate patientId matches authenticated user
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();

            return Ok(appointments);
        }
    }
}