using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminService.Data;
using AdminService.Models;
using System.Security.Claims;

namespace AdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            var admins = await _context.Admins
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
            
            return Ok(admins);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("me")]
        public async Task<ActionResult<Admin>> GetMyProfile()
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var admin = await _context.Admins
                .Include(a => a.SystemLogs)
                .FirstOrDefaultAsync(a => a.KeycloakUserId == keycloakUserId);
            
            if (admin == null) return NotFound();
            return admin;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();
            return admin;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            if (await _context.Admins.AnyAsync(a => a.Email == admin.Email))
                return BadRequest("Email already exists");

            admin.KeycloakUserId ??= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            admin.CreatedAt = DateTime.UtcNow;
            admin.UpdatedAt = DateTime.UtcNow;
            
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            await LogAction("CREATE", "Admin", $"Created admin: {admin.Name}");
            
            return CreatedAtAction(nameof(GetAdmin), new { id = admin.Id }, admin);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            if (id != admin.Id) return BadRequest();
            
            var dbAdmin = await _context.Admins.FindAsync(id);
            if (dbAdmin == null) return NotFound();

            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (dbAdmin.KeycloakUserId != keycloakUserId && !dbAdmin.IsSuperAdmin)
                return Forbid();

            dbAdmin.Name = admin.Name;
            dbAdmin.Email = admin.Email;
            dbAdmin.ContactNumber = admin.ContactNumber;
            dbAdmin.Department = admin.Department;
            dbAdmin.Privileges = admin.Privileges;
            dbAdmin.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbAdmin).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await LogAction("UPDATE", "Admin", $"Updated admin: {admin.Name}");
            
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            if (admin.IsSuperAdmin)
                return BadRequest("Cannot delete super admin");

            admin.IsActive = false;
            admin.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            await LogAction("DELETE", "Admin", $"Deactivated admin: {admin.Name}");

            return NoContent();
        }

        private async Task LogAction(string action, string entity, string details)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var log = new SystemLog
            {
                UserId = userId,
                UserRole = userRole,
                Action = action,
                Entity = entity,
                Details = details,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Timestamp = DateTime.UtcNow
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}