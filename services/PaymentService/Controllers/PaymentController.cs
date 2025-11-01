using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using System.Security.Claims;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentContext _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,receptionist")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments(
            [FromQuery] int? patientId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Payments
                .Include(p => p.Refunds)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(p => p.PatientId == patientId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            var payments = await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return Ok(payments);
        }

        [Authorize(Roles = "admin,receptionist,patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Refunds)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null) return NotFound();
            return payment;
        }

        [Authorize(Roles = "receptionist,patient")]
        [HttpPost]
        public async Task<ActionResult<Payment>> ProcessPayment(Payment payment)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            payment.ProcessedBy = userName;
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            payment.Status = "Processing";

            // Generate transaction ID
            payment.TransactionId = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Simulate payment processing
            await Task.Delay(1000);

            // Update status (in real scenario, this would be done via webhook)
            payment.Status = "Completed";
            payment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }

        [Authorize(Roles = "admin,receptionist")]
        [HttpPost("{id}/refund")]
        public async Task<ActionResult<PaymentRefund>> RefundPayment(int id, PaymentRefund refund)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound("Payment not found");

            if (payment.Status != "Completed")
                return BadRequest("Only completed payments can be refunded");

            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";

            refund.PaymentId = id;
            refund.RefundedBy = userName;
            refund.RefundDate = DateTime.UtcNow;
            refund.Status = "Completed";
            refund.TransactionId = $"RFN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

            _context.PaymentRefunds.Add(refund);

            // Update payment status if fully refunded
            var totalRefunded = await _context.PaymentRefunds
                .Where(pr => pr.PaymentId == id)
                .SumAsync(pr => pr.Amount);

            if (totalRefunded + refund.Amount >= payment.Amount)
            {
                payment.Status = "Refunded";
                payment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, refund);
        }

        [Authorize(Roles = "admin,receptionist")]
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPatientPayments(int patientId)
        {
            var payments = await _context.Payments
                .Include(p => p.Refunds)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return Ok(payments);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("report/summary")]
        public async Task<ActionResult<object>> GetPaymentSummary(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Payments.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            var summary = new
            {
                TotalPayments = await query.SumAsync(p => p.Amount),
                CompletedPayments = await query.CountAsync(p => p.Status == "Completed"),
                FailedPayments = await query.CountAsync(p => p.Status == "Failed"),
                RefundedPayments = await query.CountAsync(p => p.Status == "Refunded"),
                PaymentsByMethod = await query
                    .GroupBy(p => p.PaymentMethod)
                    .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(p => p.Amount) })
                    .ToListAsync()
            };

            return Ok(summary);
        }

        [Authorize(Roles = "admin,receptionist")]
        [HttpGet("insurance-claims")]
        public async Task<ActionResult<IEnumerable<InsuranceClaim>>> GetInsuranceClaims(
            [FromQuery] string? status)
        {
            var query = _context.InsuranceClaims.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(ic => ic.Status == status);

            var claims = await query
                .OrderByDescending(ic => ic.SubmittedDate)
                .ToListAsync();

            return Ok(claims);
        }

        [Authorize(Roles = "admin,receptionist")]
        [HttpPost("insurance-claims")]
        public async Task<ActionResult<InsuranceClaim>> SubmitInsuranceClaim(InsuranceClaim claim)
        {
            claim.ClaimNumber = $"CLM{DateTime.UtcNow:yyyyMMdd}{new Random().Next(10000, 99999)}";
            claim.Status = "Submitted";
            claim.SubmittedDate = DateTime.UtcNow;
            claim.CreatedAt = DateTime.UtcNow;
            claim.UpdatedAt = DateTime.UtcNow;

            _context.InsuranceClaims.Add(claim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInsuranceClaims), new { id = claim.Id }, claim);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("insurance-claims/{id}/approve")]
        public async Task<IActionResult> ApproveInsuranceClaim(int id, [FromBody] decimal approvedAmount)
        {
            var claim = await _context.InsuranceClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            claim.ApprovedAmount = approvedAmount;
            claim.ApprovedDate = DateTime.UtcNow;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("insurance-claims/{id}/reject")]
        public async Task<IActionResult> RejectInsuranceClaim(int id, [FromBody] string reason)
        {
            var claim = await _context.InsuranceClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            claim.RejectionReason = reason;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}