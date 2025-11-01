using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using PatientService.Models;
using System.Security.Claims;

namespace PatientService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly PatientContext _context;
        private readonly ILogger<PatientController> _logger;

        public PatientController(PatientContext context, ILogger<PatientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Patients.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search) ||
                    p.ContactNumber.Contains(search) ||
                    p.Email.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var patients = await query
                .OrderBy(p => p.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(patients);
        }

        [Authorize(Roles = "admin,patient,doctor,staff")]
        [HttpGet("me")]
        public async Task<ActionResult<Patient>> GetMyProfile()
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patient = await _context.Patients
                .Include(p => p.MedicalRecords)
                .Include(p => p.VitalSigns)
                .FirstOrDefaultAsync(p => p.KeycloakUserId == keycloakUserId);
            
            if (patient == null) return NotFound();
            return patient;
        }

        [Authorize(Roles = "admin,doctor,staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.MedicalRecords)
                .Include(p => p.VitalSigns)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (patient == null) return NotFound();
            return patient;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            if (await _context.Patients.AnyAsync(p => p.Email == patient.Email))
                return BadRequest("Email already exists");

            patient.CreatedAt = DateTime.UtcNow;
            patient.UpdatedAt = DateTime.UtcNow;
            
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        [Authorize(Roles = "admin,patient,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id) return BadRequest();
            
            var dbPatient = await _context.Patients.FindAsync(id);
            if (dbPatient == null) return NotFound();

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "patient" && dbPatient.KeycloakUserId != keycloakUserId)
                return Forbid();

            dbPatient.FirstName = patient.FirstName;
            dbPatient.LastName = patient.LastName;
            dbPatient.DOB = patient.DOB;
            dbPatient.Gender = patient.Gender;
            dbPatient.BloodGroup = patient.BloodGroup;
            dbPatient.ContactNumber = patient.ContactNumber;
            dbPatient.Address = patient.Address;
            dbPatient.Email = patient.Email;
            dbPatient.EmergencyContact = patient.EmergencyContact;
            dbPatient.EmergencyContactNumber = patient.EmergencyContactNumber;
            dbPatient.InsuranceProvider = patient.InsuranceProvider;
            dbPatient.InsuranceNumber = patient.InsuranceNumber;
            dbPatient.UpdatedAt = DateTime.UtcNow;

            if (userRole == "admin" || userRole == "staff")
            {
                dbPatient.MedicalHistory = patient.MedicalHistory;
                dbPatient.Allergies = patient.Allergies;
                dbPatient.ChronicConditions = patient.ChronicConditions;
                dbPatient.CurrentMedications = patient.CurrentMedications;
                dbPatient.Immunizations = patient.Immunizations;
            }

            _context.Entry(dbPatient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            
            patient.IsActive = false;
            patient.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "admin,doctor,staff")]
        [HttpGet("{patientId}/medical-records")]
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetPatientMedicalRecords(int patientId)
        {
            var records = await _context.MedicalRecords
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
            
            return Ok(records);
        }

        [Authorize(Roles = "admin,doctor,staff")]
        [HttpGet("{patientId}/vital-signs")]
        public async Task<ActionResult<IEnumerable<VitalSign>>> GetPatientVitalSigns(int patientId)
        {
            var vitals = await _context.VitalSigns
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();
            
            return Ok(vitals);
        }
    }
}