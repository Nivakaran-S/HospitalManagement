using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DonorService.Data;
using DonorService.Models;

namespace DonorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorController : ControllerBase
    {
        private readonly DonorContext _context;
        private readonly ILogger<DonorController> _logger;

        public DonorController(DonorContext context, ILogger<DonorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donor>>> GetDonors(
            [FromQuery] string? bloodGroup,
            [FromQuery] bool? isAvailable,
            [FromQuery] string? search)
        {
            var query = _context.Donors.Include(d => d.DonationRecords).AsQueryable();

            if (!string.IsNullOrEmpty(bloodGroup))
                query = query.Where(d => d.BloodGroup == bloodGroup);

            if (isAvailable.HasValue)
                query = query.Where(d => d.IsAvailable == isAvailable.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.Name.Contains(search) || 
                                        d.ContactNumber.Contains(search) ||
                                        d.Email.Contains(search));

            var donors = await query
                .OrderBy(d => d.Name)
                .ToListAsync();

            return Ok(donors);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Donor>> GetDonor(int id)
        {
            var donor = await _context.Donors
                .Include(d => d.DonationRecords)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donor == null) return NotFound();
            return donor;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<ActionResult<Donor>> PostDonor(Donor donor)
        {
            if (await _context.Donors.AnyAsync(d => d.Email == donor.Email))
                return BadRequest("Email already exists");

            donor.CreatedAt = DateTime.UtcNow;
            donor.UpdatedAt = DateTime.UtcNow;

            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDonor), new { id = donor.Id }, donor);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDonor(int id, Donor donor)
        {
            if (id != donor.Id) return BadRequest();

            var dbDonor = await _context.Donors.FindAsync(id);
            if (dbDonor == null) return NotFound();

            dbDonor.Name = donor.Name;
            dbDonor.BloodGroup = donor.BloodGroup;
            dbDonor.ContactNumber = donor.ContactNumber;
            dbDonor.Email = donor.Email;
            dbDonor.DateOfBirth = donor.DateOfBirth;
            dbDonor.Gender = donor.Gender;
            dbDonor.Address = donor.Address;
            dbDonor.Weight = donor.Weight;
            dbDonor.IsAvailable = donor.IsAvailable;
            dbDonor.MedicalHistory = donor.MedicalHistory;
            dbDonor.EmergencyContact = donor.EmergencyContact;
            dbDonor.EmergencyContactNumber = donor.EmergencyContactNumber;
            dbDonor.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbDonor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Donors.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return NotFound();

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("blood-group/{bloodGroup}")]
        public async Task<ActionResult<IEnumerable<Donor>>> GetDonorsByBloodGroup(string bloodGroup)
        {
            var donors = await _context.Donors
                .Where(d => d.BloodGroup == bloodGroup && d.IsAvailable)
                .OrderByDescending(d => d.LastDonationDate)
                .ToListAsync();

            return Ok(donors);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("eligible")]
        public async Task<ActionResult<IEnumerable<Donor>>> GetEligibleDonors()
        {
            // Donors eligible if last donation was more than 3 months ago
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
            
            var donors = await _context.Donors
                .Where(d => d.IsAvailable && d.LastDonationDate < threeMonthsAgo)
                .OrderBy(d => d.LastDonationDate)
                .ToListAsync();

            return Ok(donors);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{id}/record-donation")]
        public async Task<ActionResult<DonationRecord>> RecordDonation(int id, DonationRecord record)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return NotFound("Donor not found");

            record.DonorId = id;
            record.DonationDate = DateTime.UtcNow;

            _context.DonationRecords.Add(record);

            // Update donor's last donation date
            donor.LastDonationDate = DateTime.UtcNow;
            donor.UpdatedAt = DateTime.UtcNow;

            // Update blood inventory if donation is safe
            if (record.IsSafe)
            {
                var inventory = await _context.BloodInventories
                    .FirstOrDefaultAsync(bi => bi.BloodGroup == record.BloodGroup);

                if (inventory != null)
                {
                    inventory.UnitsAvailable += record.UnitsCollected;
                    inventory.LastUpdated = DateTime.UtcNow;
                }
                else
                {
                    _context.BloodInventories.Add(new BloodInventory
                    {
                        BloodGroup = record.BloodGroup,
                        UnitsAvailable = record.UnitsCollected,
                        LastUpdated = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddDays(42) // Blood typically lasts 42 days
                    });
                }
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDonor), new { id = donor.Id }, record);
        }
    }
}