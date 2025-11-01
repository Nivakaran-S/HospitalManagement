using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoctorService.Data;
using DoctorService.Models;
using System.Security.Claims;

namespace DoctorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorAvailabilityController : ControllerBase
    {
        private readonly DoctorContext _context;
        private readonly ILogger<DoctorAvailabilityController> _logger;

        public DoctorAvailabilityController(DoctorContext context, ILogger<DoctorAvailabilityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost]
        public async Task<ActionResult<DoctorAvailability>> CreateAvailability(DoctorAvailability availability)
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.KeycloakUserId == keycloakUserId);

            if (doctor == null) return NotFound("Doctor not found");
            
            if (availability.DoctorId != doctor.Id)
                return Forbid();

            // Check for overlapping availability
            var overlap = await _context.DoctorAvailabilities
                .AnyAsync(da => da.DoctorId == availability.DoctorId &&
                               da.DayOfWeek == availability.DayOfWeek &&
                               da.IsAvailable &&
                               ((availability.StartTime >= da.StartTime && availability.StartTime < da.EndTime) ||
                                (availability.EndTime > da.StartTime && availability.EndTime <= da.EndTime)));

            if (overlap)
                return BadRequest("Availability overlaps with existing schedule");

            _context.DoctorAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAvailability), new { id = availability.Id }, availability);
        }

        [Authorize(Roles = "admin,doctor,staff,patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorAvailability>> GetAvailability(int id)
        {
            var availability = await _context.DoctorAvailabilities
                .Include(da => da.Doctor)
                .FirstOrDefaultAsync(da => da.Id == id);

            if (availability == null) return NotFound();
            return availability;
        }

        [Authorize(Roles = "doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailability(int id, DoctorAvailability availability)
        {
            if (id != availability.Id) return BadRequest();

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.KeycloakUserId == keycloakUserId);

            if (doctor == null) return NotFound("Doctor not found");

            var dbAvailability = await _context.DoctorAvailabilities.FindAsync(id);
            if (dbAvailability == null) return NotFound();

            if (dbAvailability.DoctorId != doctor.Id)
                return Forbid();

            dbAvailability.DayOfWeek = availability.DayOfWeek;
            dbAvailability.StartTime = availability.StartTime;
            dbAvailability.EndTime = availability.EndTime;
            dbAvailability.SlotDurationMinutes = availability.SlotDurationMinutes;
            dbAvailability.IsAvailable = availability.IsAvailable;

            _context.Entry(dbAvailability).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.KeycloakUserId == keycloakUserId);

            if (doctor == null) return NotFound("Doctor not found");

            var availability = await _context.DoctorAvailabilities.FindAsync(id);
            if (availability == null) return NotFound();

            if (availability.DoctorId != doctor.Id)
                return Forbid();

            _context.DoctorAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,patient,staff")]
        [HttpGet("available-slots/{doctorId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableSlots(
            int doctorId, 
            [FromQuery] DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;
            var availabilities = await _context.DoctorAvailabilities
                .Where(da => da.DoctorId == doctorId && 
                            da.DayOfWeek == dayOfWeek && 
                            da.IsAvailable)
                .ToListAsync();

            if (!availabilities.Any())
                return Ok(new List<object>());

            var slots = new List<object>();
            foreach (var availability in availabilities)
            {
                var currentTime = availability.StartTime;
                while (currentTime.Add(TimeSpan.FromMinutes(availability.SlotDurationMinutes)) <= availability.EndTime)
                {
                    slots.Add(new
                    {
                        Time = currentTime.ToString(@"hh\:mm"),
                        DateTime = date.Date.Add(currentTime),
                        DurationMinutes = availability.SlotDurationMinutes
                    });
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(availability.SlotDurationMinutes));
                }
            }

            return Ok(slots);
        }
    }
}