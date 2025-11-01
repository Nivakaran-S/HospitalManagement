using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DonorService.Data;
using DonorService.Models;

namespace DonorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloodInventoryController : ControllerBase
    {
        private readonly DonorContext _context;
        private readonly ILogger<BloodInventoryController> _logger;

        public BloodInventoryController(DonorContext context, ILogger<BloodInventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodInventory>>> GetBloodInventories()
        {
            var inventories = await _context.BloodInventories
                .OrderBy(bi => bi.BloodGroup)
                .ToListAsync();

            return Ok(inventories);
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("{bloodGroup}")]
        public async Task<ActionResult<BloodInventory>> GetBloodInventory(string bloodGroup)
        {
            var inventory = await _context.BloodInventories
                .FirstOrDefaultAsync(bi => bi.BloodGroup == bloodGroup);

            if (inventory == null)
                return Ok(new BloodInventory { BloodGroup = bloodGroup, UnitsAvailable = 0 });

            return inventory;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<ActionResult<BloodInventory>> PostBloodInventory(BloodInventory inventory)
        {
            var existing = await _context.BloodInventories
                .FirstOrDefaultAsync(bi => bi.BloodGroup == inventory.BloodGroup);

            if (existing != null)
                return BadRequest("Blood group inventory already exists. Use PUT to update.");

            inventory.LastUpdated = DateTime.UtcNow;
            _context.BloodInventories.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBloodInventory), 
                new { bloodGroup = inventory.BloodGroup }, inventory);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBloodInventory(int id, BloodInventory inventory)
        {
            if (id != inventory.Id) return BadRequest();

            var dbInventory = await _context.BloodInventories.FindAsync(id);
            if (dbInventory == null) return NotFound();

            dbInventory.UnitsAvailable = inventory.UnitsAvailable;
            dbInventory.Location = inventory.Location;
            dbInventory.ExpiryDate = inventory.ExpiryDate;
            dbInventory.LastUpdated = DateTime.UtcNow;

            _context.Entry(dbInventory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{bloodGroup}/use")]
        public async Task<IActionResult> UseBlood(string bloodGroup, [FromBody] int units)
        {
            var inventory = await _context.BloodInventories
                .FirstOrDefaultAsync(bi => bi.BloodGroup == bloodGroup);

            if (inventory == null)
                return NotFound("Blood group not found in inventory");

            if (inventory.UnitsAvailable < units)
                return BadRequest("Insufficient blood units available");

            inventory.UnitsAvailable -= units;
            inventory.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<BloodInventory>>> GetLowStockAlerts([FromQuery] int threshold = 1000)
        {
            var lowStock = await _context.BloodInventories
                .Where(bi => bi.UnitsAvailable < threshold)
                .OrderBy(bi => bi.UnitsAvailable)
                .ToListAsync();

            return Ok(lowStock);
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("expiring-soon")]
        public async Task<ActionResult<IEnumerable<BloodInventory>>> GetExpiringSoon([FromQuery] int days = 7)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var expiring = await _context.BloodInventories
                .Where(bi => bi.ExpiryDate <= expiryDate && bi.UnitsAvailable > 0)
                .OrderBy(bi => bi.ExpiryDate)
                .ToListAsync();

            return Ok(expiring);
        }
    }
}