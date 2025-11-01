using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabService.Data;
using LabService.Models;

namespace LabService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabTestController : ControllerBase
    {
        private readonly LabTestContext _context;
        private readonly ILogger<LabTestController> _logger;

        public LabTestController(LabTestContext context, ILogger<LabTestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabTest>>> GetLabTests(
            [FromQuery] int? patientId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.LabTests
                .Include(lt => lt.TestResults)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(lt => lt.PatientId == patientId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(lt => lt.Status == status);

            if (fromDate.HasValue)
                query = query.Where(lt => lt.OrderedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(lt => lt.OrderedDate <= toDate.Value);

            var tests = await query
                .OrderByDescending(lt => lt.OrderedDate)
                .ToListAsync();

            return Ok(tests);
        }

        [Authorize(Roles = "admin,staff,doctor,patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<LabTest>> GetLabTest(int id)
        {
            var labTest = await _context.LabTests
                .Include(lt => lt.TestResults)
                .FirstOrDefaultAsync(lt => lt.Id == id);

            if (labTest == null) return NotFound();
            return labTest;
        }

        [Authorize(Roles = "doctor,staff")]
        [HttpPost]
        public async Task<ActionResult<LabTest>> PostLabTest(LabTest labTest)
        {
            labTest.CreatedAt = DateTime.UtcNow;
            labTest.UpdatedAt = DateTime.UtcNow;
            labTest.Status = "Pending";

            _context.LabTests.Add(labTest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLabTest), new { id = labTest.Id }, labTest);
        }

        [Authorize(Roles = "staff,doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLabTest(int id, LabTest labTest)
        {
            if (id != labTest.Id) return BadRequest();

            var dbLabTest = await _context.LabTests.FindAsync(id);
            if (dbLabTest == null) return NotFound();

            dbLabTest.TestDate = labTest.TestDate;
            dbLabTest.Result = labTest.Result;
            dbLabTest.Status = labTest.Status;
            dbLabTest.Notes = labTest.Notes;
            dbLabTest.TechnicianName = labTest.TechnicianName;
            dbLabTest.UpdatedAt = DateTime.UtcNow;

            if (labTest.Status == "Completed")
            {
                dbLabTest.CompletedDate = DateTime.UtcNow;
            }

            _context.Entry(dbLabTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LabTests.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabTest(int id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest == null) return NotFound();

            if (labTest.Status == "Completed")
                return BadRequest("Cannot delete completed lab test");

            labTest.Status = "Cancelled";
            labTest.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "staff")]
        [HttpPost("{id}/results")]
        public async Task<ActionResult<LabTestResult>> AddTestResult(int id, LabTestResult result)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest == null) return NotFound("Lab test not found");

            result.LabTestId = id;
            _context.LabTestResults.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLabTest), new { id = labTest.Id }, result);
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<LabTest>>> GetPatientLabTests(int patientId)
        {
            var tests = await _context.LabTests
                .Include(lt => lt.TestResults)
                .Where(lt => lt.PatientId == patientId)
                .OrderByDescending(lt => lt.OrderedDate)
                .ToListAsync();

            return Ok(tests);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<LabTest>>> GetPendingTests()
        {
            var tests = await _context.LabTests
                .Where(lt => lt.Status == "Pending" || lt.Status == "InProgress")
                .OrderByDescending(lt => lt.IsUrgent)
                .ThenBy(lt => lt.OrderedDate)
                .ToListAsync();

            return Ok(tests);
        }

        [Authorize(Roles = "staff")]
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartTest(int id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest == null) return NotFound();

            if (labTest.Status != "Pending")
                return BadRequest("Test must be in Pending status");

            labTest.Status = "InProgress";
            labTest.TestDate = DateTime.UtcNow;
            labTest.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "staff")]
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTest(int id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest == null) return NotFound();

            if (labTest.Status == "Completed")
                return BadRequest("Test is already completed");

            labTest.Status = "Completed";
            labTest.CompletedDate = DateTime.UtcNow;
            labTest.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("report/summary")]
        public async Task<ActionResult<object>> GetLabSummary(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.LabTests.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(lt => lt.OrderedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(lt => lt.OrderedDate <= toDate.Value);

            var summary = new
            {
                TotalTests = await query.CountAsync(),
                PendingTests = await query.CountAsync(lt => lt.Status == "Pending"),
                InProgressTests = await query.CountAsync(lt => lt.Status == "InProgress"),
                CompletedTests = await query.CountAsync(lt => lt.Status == "Completed"),
                UrgentTests = await query.CountAsync(lt => lt.IsUrgent && lt.Status != "Completed"),
                TestsByCategory = await query
                    .GroupBy(lt => lt.TestCategory)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToListAsync()
            };

            return Ok(summary);
        }
    }
}