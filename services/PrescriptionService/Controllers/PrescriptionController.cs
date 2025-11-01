using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrescriptionService.Data;
using PrescriptionService.Models;

namespace PrescriptionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly PrescriptionContext _context;
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(PrescriptionContext context, ILogger<PrescriptionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions(
            [FromQuery] int? patientId,
            [FromQuery] int? doctorId,
            [FromQuery] string? status)
        {
            var query = _context.Prescriptions
                .Include(p => p.Medicines)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(p => p.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(p => p.DoctorId == doctorId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status);

            var prescriptions = await query
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();

            return Ok(prescriptions);
        }

        [Authorize(Roles = "admin,staff,doctor,patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Medicines)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null) return NotFound();
            return prescription;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost]
        public async Task<ActionResult<Prescription>> PostPrescription(Prescription prescription)
        {
            prescription.CreatedAt = DateTime.UtcNow;
            prescription.UpdatedAt = DateTime.UtcNow;
            prescription.Status = "Pending";

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
        }

        [Authorize(Roles = "doctor,pharmacist")]
        [HttpPost("{id}/send-to-pharmacy")]
        public async Task<IActionResult> SendToPharmacy(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            if (prescription.Status != "Pending")
                return BadRequest("Only pending prescriptions can be sent to pharmacy");

            prescription.Status = "InPharmacy";
            prescription.SentToPharmacyAt = DateTime.UtcNow;
            prescription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescription(int id, Prescription prescription)
        {
            if (id != prescription.Id) return BadRequest();

            var dbPrescription = await _context.Prescriptions.FindAsync(id);
            if (dbPrescription == null) return NotFound();

            dbPrescription.Diagnosis = prescription.Diagnosis;
            dbPrescription.Notes = prescription.Notes;
            dbPrescription.Status = prescription.Status;
            dbPrescription.ValidUntil = prescription.ValidUntil;
            dbPrescription.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbPrescription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Prescriptions.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            prescription.Status = "Cancelled";
            prescription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("{id}/medicines")]
        public async Task<ActionResult<PrescriptionMedicine>> AddMedicine(int id, PrescriptionMedicine medicine)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound("Prescription not found");

            medicine.PrescriptionId = id;
            _context.PrescriptionMedicines.Add(medicine);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, medicine);
        }

        [Authorize(Roles = "staff")]
        [HttpPost("medicine/{medicineId}/dispense")]
        public async Task<IActionResult> DispenseMedicine(int medicineId, [FromBody] string dispensedBy)
        {
            var medicine = await _context.PrescriptionMedicines.FindAsync(medicineId);
            if (medicine == null) return NotFound();

            if (medicine.IsDispensed)
                return BadRequest("Medicine already dispensed");

            medicine.IsDispensed = true;
            medicine.DispensedDate = DateTime.UtcNow;
            medicine.DispensedBy = dispensedBy;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPatientPrescriptions(int patientId)
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Medicines)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();

            return Ok(prescriptions);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("pending-dispensing")]
        public async Task<ActionResult<IEnumerable<PrescriptionMedicine>>> GetPendingDispensing()
        {
            var medicines = await _context.PrescriptionMedicines
                .Include(pm => pm.Prescription)
                .Where(pm => !pm.IsDispensed)
                .OrderBy(pm => pm.Prescription.PrescribedDate)
                .ToListAsync();

            return Ok(medicines);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("report/summary")]
        public async Task<ActionResult<object>> GetPrescriptionSummary(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Prescriptions.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.PrescribedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PrescribedDate <= toDate.Value);

            var summary = new
            {
                TotalPrescriptions = await query.CountAsync(),
                ActivePrescriptions = await query.CountAsync(p => p.Status == "Active"),
                CompletedPrescriptions = await query.CountAsync(p => p.Status == "Completed"),
                PendingDispensing = await _context.PrescriptionMedicines
                    .CountAsync(pm => !pm.IsDispensed)
            };

            return Ok(summary);
        }
    }
}