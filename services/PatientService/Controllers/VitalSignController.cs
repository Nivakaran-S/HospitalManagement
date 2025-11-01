using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using PatientService.Models;

namespace PatientService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VitalSignController : ControllerBase
    {
        private readonly PatientContext _context;
        private readonly ILogger<VitalSignController> _logger;

        public VitalSignController(PatientContext context, ILogger<VitalSignController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "doctor,staff")]
        [HttpPost]
        public async Task<ActionResult<VitalSign>> CreateVitalSign(VitalSign vitalSign)
        {
            var patient = await _context.Patients.FindAsync(vitalSign.PatientId);
            if (patient == null) return NotFound("Patient not found");

            vitalSign.RecordedAt = DateTime.UtcNow;
            
            _context.VitalSigns.Add(vitalSign);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVitalSign), new { id = vitalSign.Id }, vitalSign);
        }

        [Authorize(Roles = "admin,doctor,staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VitalSign>> GetVitalSign(int id)
        {
            var vitalSign = await _context.VitalSigns
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vitalSign == null) return NotFound();
            return vitalSign;
        }

        [Authorize(Roles = "doctor,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVitalSign(int id, VitalSign vitalSign)
        {
            if (id != vitalSign.Id) return BadRequest();

            var dbVitalSign = await _context.VitalSigns.FindAsync(id);
            if (dbVitalSign == null) return NotFound();

            dbVitalSign.Temperature = vitalSign.Temperature;
            dbVitalSign.BloodPressure = vitalSign.BloodPressure;
            dbVitalSign.HeartRate = vitalSign.HeartRate;
            dbVitalSign.RespiratoryRate = vitalSign.RespiratoryRate;
            dbVitalSign.OxygenSaturation = vitalSign.OxygenSaturation;
            dbVitalSign.Height = vitalSign.Height;
            dbVitalSign.Weight = vitalSign.Weight;
            dbVitalSign.Notes = vitalSign.Notes;

            _context.Entry(dbVitalSign).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVitalSign(int id)
        {
            var vitalSign = await _context.VitalSigns.FindAsync(id);
            if (vitalSign == null) return NotFound();

            _context.VitalSigns.Remove(vitalSign);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}