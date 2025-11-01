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
    public class DoctorController : ControllerBase
    {
        private readonly DoctorContext _context;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(DoctorContext context, ILogger<DoctorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,doctor,staff,patient")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors(
            [FromQuery] string? speciality,
            [FromQuery] string? search,
            [FromQuery] bool activeOnly = true)
        {
            var query = _context.Doctors.AsQueryable();

            if (activeOnly)
                query = query.Where(d => d.IsActive);

            if (!string.IsNullOrEmpty(speciality))
                query = query.Where(d => d.Speciality.Contains(speciality));

            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.Name.Contains(search) || d.Department.Contains(search));

            var doctors = await query
                .Include(d => d.Availabilities)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return Ok(doctors);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpGet("me")]
        public async Task<ActionResult<Doctor>> GetMyProfile()
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors
                .Include(d => d.Availabilities)
                .FirstOrDefaultAsync(d => d.KeycloakUserId == keycloakUserId);
            
            if (doctor == null) return NotFound();
            return doctor;
        }

        [Authorize(Roles = "admin,doctor,staff,patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Availabilities)
                .FirstOrDefaultAsync(d => d.Id == id);
            
            if (doctor == null) return NotFound();
            return doctor;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            if (await _context.Doctors.AnyAsync(d => d.Email == doctor.Email))
                return BadRequest("Email already exists");

            doctor.CreatedAt = DateTime.UtcNow;
            doctor.UpdatedAt = DateTime.UtcNow;
            
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.Id) return BadRequest();
            
            var dbDoctor = await _context.Doctors.FindAsync(id);
            if (dbDoctor == null) return NotFound();

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "doctor" && dbDoctor.KeycloakUserId != keycloakUserId)
                return Forbid();

            dbDoctor.Name = doctor.Name;
            dbDoctor.Speciality = doctor.Speciality;
            dbDoctor.ContactNumber = doctor.ContactNumber;
            dbDoctor.Email = doctor.Email;
            dbDoctor.Department = doctor.Department;
            dbDoctor.Qualifications = doctor.Qualifications;
            dbDoctor.ExperienceYears = doctor.ExperienceYears;
            dbDoctor.LicenseNumber = doctor.LicenseNumber;
            dbDoctor.ConsultationFee = doctor.ConsultationFee;
            dbDoctor.UpdatedAt = DateTime.UtcNow;

            if (userRole == "admin")
            {
                dbDoctor.IsActive = doctor.IsActive;
            }

            _context.Entry(dbDoctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            
            doctor.IsActive = false;
            doctor.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "admin,doctor,patient,staff")]
        [HttpGet("{doctorId}/availabilities")]
        public async Task<ActionResult<IEnumerable<DoctorAvailability>>> GetDoctorAvailabilities(int doctorId)
        {
            var availabilities = await _context.DoctorAvailabilities
                .Where(da => da.DoctorId == doctorId && da.IsAvailable)
                .OrderBy(da => da.DayOfWeek)
                .ThenBy(da => da.StartTime)
                .ToListAsync();

            return Ok(availabilities);
        }
    }
}