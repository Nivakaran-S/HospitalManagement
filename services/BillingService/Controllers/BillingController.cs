using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BillingService.Data;
using BillingService.Models;

namespace BillingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly BillingContext _context;
        private readonly ILogger<BillingController> _logger;

        public BillingController(BillingContext context, ILogger<BillingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Billing>>> GetBillings(
            [FromQuery] int? patientId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Billings
                .Include(b => b.BillingItems)
                .Include(b => b.PaymentTransactions)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(b => b.PatientId == patientId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.Status == status);

            if (fromDate.HasValue)
                query = query.Where(b => b.BillingDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(b => b.BillingDate <= toDate.Value);

            var billings = await query
                .OrderByDescending(b => b.BillingDate)
                .ToListAsync();

            return Ok(billings);
        }

        [Authorize(Roles = "admin,staff,patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Billing>> GetBilling(int id)
        {
            var billing = await _context.Billings
                .Include(b => b.BillingItems)
                .Include(b => b.PaymentTransactions)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billing == null) return NotFound();
            return billing;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpPost]
        public async Task<ActionResult<Billing>> PostBilling(Billing billing)
        {
            billing.CreatedAt = DateTime.UtcNow;
            billing.UpdatedAt = DateTime.UtcNow;
            billing.DueAmount = billing.Amount - billing.PaidAmount;

            if (billing.PaidAmount >= billing.Amount)
            {
                billing.Status = "Paid";
                billing.PaidDate = DateTime.UtcNow;
            }
            else if (billing.PaidAmount > 0)
            {
                billing.Status = "PartiallyPaid";
            }

            _context.Billings.Add(billing);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBilling), new { id = billing.Id }, billing);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBilling(int id, Billing billing)
        {
            if (id != billing.Id) return BadRequest();

            var dbBilling = await _context.Billings.FindAsync(id);
            if (dbBilling == null) return NotFound();

            dbBilling.Description = billing.Description;
            dbBilling.Amount = billing.Amount;
            dbBilling.Status = billing.Status;
            dbBilling.UpdatedAt = DateTime.UtcNow;
            dbBilling.DueAmount = billing.Amount - dbBilling.PaidAmount;

            _context.Entry(dbBilling).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Billings.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilling(int id)
        {
            var billing = await _context.Billings.FindAsync(id);
            if (billing == null) return NotFound();

            _context.Billings.Remove(billing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{id}/items")]
        public async Task<ActionResult<BillingItem>> AddBillingItem(int id, BillingItem item)
        {
            var billing = await _context.Billings.FindAsync(id);
            if (billing == null) return NotFound("Billing not found");

            item.BillingId = id;
            item.TotalPrice = item.Quantity * item.UnitPrice;

            _context.BillingItems.Add(item);

            // Update billing total
            billing.Amount += item.TotalPrice;
            billing.DueAmount = billing.Amount - billing.PaidAmount;
            billing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBilling), new { id = billing.Id }, item);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{id}/payments")]
        public async Task<ActionResult<PaymentTransaction>> RecordPayment(int id, PaymentTransaction payment)
        {
            var billing = await _context.Billings.FindAsync(id);
            if (billing == null) return NotFound("Billing not found");

            payment.BillingId = id;
            payment.PaymentDate = DateTime.UtcNow;

            _context.PaymentTransactions.Add(payment);

            // Update billing payment status
            billing.PaidAmount += payment.Amount;
            billing.DueAmount = billing.Amount - billing.PaidAmount;

            if (billing.PaidAmount >= billing.Amount)
            {
                billing.Status = "Paid";
                billing.PaidDate = DateTime.UtcNow;
            }
            else if (billing.PaidAmount > 0)
            {
                billing.Status = "PartiallyPaid";
            }

            billing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBilling), new { id = billing.Id }, payment);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Billing>>> GetPatientBillings(int patientId)
        {
            var billings = await _context.Billings
                .Include(b => b.BillingItems)
                .Include(b => b.PaymentTransactions)
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.BillingDate)
                .ToListAsync();

            return Ok(billings);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Billing>>> GetPendingBillings()
        {
            var billings = await _context.Billings
                .Include(b => b.BillingItems)
                .Where(b => b.Status == "Unpaid" || b.Status == "PartiallyPaid")
                .OrderBy(b => b.BillingDate)
                .ToListAsync();

            return Ok(billings);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("report/summary")]
        public async Task<ActionResult<object>> GetBillingSummary(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Billings.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(b => b.BillingDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(b => b.BillingDate <= toDate.Value);

            var summary = new
            {
                TotalBilled = await query.SumAsync(b => b.Amount),
                TotalPaid = await query.SumAsync(b => b.PaidAmount),
                TotalDue = await query.SumAsync(b => b.DueAmount),
                PaidCount = await query.CountAsync(b => b.Status == "Paid"),
                UnpaidCount = await query.CountAsync(b => b.Status == "Unpaid"),
                PartiallyPaidCount = await query.CountAsync(b => b.Status == "PartiallyPaid")
            };

            return Ok(summary);
        }
    }
}