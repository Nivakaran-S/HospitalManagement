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
    public class MedicalRecordController : ControllerBase
    {
        private readonly PatientContext _context;
        private readonly ILogger<MedicalRecordController> _logger;

        public MedicalRecordController(PatientContext context, ILogger<MedicalRecordController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> CreateMedicalRecord(MedicalRecord record)
        {
            var patient = await _context.Patients.FindAsync(record.PatientId);
            if (patient == null) return NotFound("Patient not found");

            record.CreatedAt = DateTime.UtcNow;
            
            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = record.Id }, record);
        }

        [Authorize(Roles = "admin,doctor,staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (record == null) return NotFound();
            return record;
        }

        [Authorize(Roles = "doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, MedicalRecord record)
        {
            if (id != record.Id) return BadRequest();

            var dbRecord = await _context.MedicalRecords.FindAsync(id);
            if (dbRecord == null) return NotFound();

            dbRecord.ChiefComplaint = record.ChiefComplaint;
            dbRecord.Diagnosis = record.Diagnosis;
            dbRecord.Treatment = record.Treatment;
            dbRecord.Notes = record.Notes;
            dbRecord.PrescribedMedications = record.PrescribedMedications;
            dbRecord.FollowUpInstructions = record.FollowUpInstructions;
            dbRecord.FollowUpDate = record.FollowUpDate;
            dbRecord.ConsultationFee = record.ConsultationFee;

            _context.Entry(dbRecord).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null) return NotFound();

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}