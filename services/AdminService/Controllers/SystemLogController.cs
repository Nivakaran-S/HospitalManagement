using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminService.Data;
using AdminService.Models;

namespace AdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemLogController : ControllerBase
    {
        private readonly AdminContext _context;
        private readonly ILogger<SystemLogController> _logger;

        public SystemLogController(AdminContext context, ILogger<SystemLogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemLog>>> GetSystemLogs(
            [FromQuery] string? userId,
            [FromQuery] string? action,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.SystemLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(sl => sl.UserId == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(sl => sl.Action == action);

            if (fromDate.HasValue)
                query = query.Where(sl => sl.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(sl => sl.Timestamp <= toDate.Value);

            var totalCount = await query.CountAsync();
            var logs = await query
                .OrderByDescending(sl => sl.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(logs);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemLog>> GetSystemLog(int id)
        {
            var log = await _context.SystemLogs.FindAsync(id);
            if (log == null) return NotFound();
            return log;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("activity-summary")]
        public async Task<ActionResult<object>> GetActivitySummary(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.SystemLogs.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(sl => sl.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(sl => sl.Timestamp <= toDate.Value);

            var summary = new
            {
                TotalActions = await query.CountAsync(),
                ActionsByType = await query
                    .GroupBy(sl => sl.Action)
                    .Select(g => new { Action = g.Key, Count = g.Count() })
                    .ToListAsync(),
                ActionsByUser = await query
                    .GroupBy(sl => sl.UserId)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync(),
                ActionsByEntity = await query
                    .GroupBy(sl => sl.Entity)
                    .Select(g => new { Entity = g.Key, Count = g.Count() })
                    .ToListAsync()
            };

            return Ok(summary);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupOldLogs([FromQuery] int daysToKeep = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var oldLogs = await _context.SystemLogs
                .Where(sl => sl.Timestamp < cutoffDate)
                .ToListAsync();

            _context.SystemLogs.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();

            return Ok(new { DeletedCount = oldLogs.Count });
        }
    }
}