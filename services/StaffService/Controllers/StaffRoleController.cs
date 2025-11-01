using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffService.Data;
using StaffService.Models;
using System.Security.Claims;

namespace StaffService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffRoleController : ControllerBase
    {
        private readonly StaffContext _context;
        private readonly ILogger<StaffRoleController> _logger;

        public StaffRoleController(StaffContext context, ILogger<StaffRoleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffRole>>> GetStaffRoles([FromQuery] bool includeInactive = false)
        {
            var query = _context.StaffRoles
                .Include(sr => sr.Permissions)
                .AsQueryable();

            if (!includeInactive)
                query = query.Where(sr => sr.IsActive);

            var roles = await query
                .OrderBy(sr => sr.RoleName)
                .ToListAsync();

            return Ok(roles);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffRole>> GetStaffRole(int id)
        {
            var role = await _context.StaffRoles
                .Include(sr => sr.Permissions)
                .Include(sr => sr.StaffMembers)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (role == null) return NotFound();
            return role;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<StaffRole>> CreateStaffRole(StaffRole role)
        {
            if (await _context.StaffRoles.AnyAsync(sr => sr.RoleName == role.RoleName))
                return BadRequest("Role name already exists");

            if (await _context.StaffRoles.AnyAsync(sr => sr.KeycloakRoleName == role.KeycloakRoleName))
                return BadRequest("Keycloak role name already exists");

            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
            role.CreatedBy = userName;
            role.CreatedAt = DateTime.UtcNow;

            _context.StaffRoles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStaffRole), new { id = role.Id }, role);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaffRole(int id, StaffRole role)
        {
            if (id != role.Id) return BadRequest();

            var dbRole = await _context.StaffRoles.FindAsync(id);
            if (dbRole == null) return NotFound();

            // Check for duplicate names
            if (await _context.StaffRoles.AnyAsync(sr => sr.RoleName == role.RoleName && sr.Id != id))
                return BadRequest("Role name already exists");

            dbRole.RoleName = role.RoleName;
            dbRole.Description = role.Description;
            dbRole.KeycloakRoleName = role.KeycloakRoleName;
            dbRole.IsActive = role.IsActive;

            _context.Entry(dbRole).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaffRole(int id)
        {
            var role = await _context.StaffRoles
                .Include(sr => sr.StaffMembers)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (role == null) return NotFound();

            if (role.StaffMembers.Any())
                return BadRequest("Cannot delete role with assigned staff members. Deactivate instead.");

            _context.StaffRoles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{id}/permissions")]
        public async Task<ActionResult<StaffRolePermission>> AddPermission(int id, StaffRolePermission permission)
        {
            var role = await _context.StaffRoles.FindAsync(id);
            if (role == null) return NotFound("Role not found");

            // Check if permission for this module already exists
            if (await _context.StaffRolePermissions.AnyAsync(
                srp => srp.StaffRoleId == id && srp.Module == permission.Module))
                return BadRequest("Permission for this module already exists. Use PUT to update.");

            permission.StaffRoleId = id;
            _context.StaffRolePermissions.Add(permission);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStaffRole), new { id = role.Id }, permission);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("permissions/{permissionId}")]
        public async Task<IActionResult> UpdatePermission(int permissionId, StaffRolePermission permission)
        {
            if (permissionId != permission.Id) return BadRequest();

            var dbPermission = await _context.StaffRolePermissions.FindAsync(permissionId);
            if (dbPermission == null) return NotFound();

            dbPermission.Module = permission.Module;
            dbPermission.CanView = permission.CanView;
            dbPermission.CanCreate = permission.CanCreate;
            dbPermission.CanUpdate = permission.CanUpdate;
            dbPermission.CanDelete = permission.CanDelete;
            dbPermission.AdditionalPermissions = permission.AdditionalPermissions;

            _context.Entry(dbPermission).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("permissions/{permissionId}")]
        public async Task<IActionResult> DeletePermission(int permissionId)
        {
            var permission = await _context.StaffRolePermissions.FindAsync(permissionId);
            if (permission == null) return NotFound();

            _context.StaffRolePermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IEnumerable<StaffRolePermission>>> GetRolePermissions(int id)
        {
            var permissions = await _context.StaffRolePermissions
                .Where(srp => srp.StaffRoleId == id)
                .OrderBy(srp => srp.Module)
                .ToListAsync();

            return Ok(permissions);
        }

        [Authorize]
        [HttpGet("my-permissions")]
        public async Task<ActionResult<object>> GetMyPermissions()
        {
            var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var staff = await _context.StaffMembers
                .Include(s => s.StaffRole)
                    .ThenInclude(sr => sr.Permissions)
                .FirstOrDefaultAsync(s => s.KeycloakUserId == keycloakUserId);

            if (staff == null)
                return NotFound("Staff member not found");

            var permissions = staff.StaffRole.Permissions
                .GroupBy(p => p.Module)
                .ToDictionary(
                    g => g.Key,
                    g => g.First()
                );

            return Ok(new
            {
                RoleName = staff.StaffRole.RoleName,
                Permissions = permissions
            });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("initialize-default-roles")]
        public async Task<IActionResult> InitializeDefaultRoles()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var defaultRoles = new[]
            {
                new StaffRole
                {
                    RoleName = "Pharmacist",
                    Description = "Manages pharmacy, dispenses medicines",
                    KeycloakRoleName = "pharmacist",
                    CreatedBy = userName
                },
                new StaffRole
                {
                    RoleName = "Receptionist",
                    Description = "Manages appointments, patient registration",
                    KeycloakRoleName = "receptionist",
                    CreatedBy = userName
                },
                new StaffRole
                {
                    RoleName = "Nurse",
                    Description = "Records vital signs, assists doctors",
                    KeycloakRoleName = "nurse",
                    CreatedBy = userName
                },
                new StaffRole
                {
                    RoleName = "Lab Technician",
                    Description = "Conducts lab tests and records results",
                    KeycloakRoleName = "lab_technician",
                    CreatedBy = userName
                }
            };

            foreach (var role in defaultRoles)
            {
                if (!await _context.StaffRoles.AnyAsync(sr => sr.RoleName == role.RoleName))
                {
                    _context.StaffRoles.Add(role);
                }
            }

            await _context.SaveChangesAsync();

            // Add default permissions
            await InitializeDefaultPermissions();

            return Ok("Default roles initialized");
        }

        private async Task InitializeDefaultPermissions()
        {
            // Pharmacist permissions
            var pharmacistRole = await _context.StaffRoles.FirstOrDefaultAsync(sr => sr.RoleName == "Pharmacist");
            if (pharmacistRole != null && !await _context.StaffRolePermissions.AnyAsync(p => p.StaffRoleId == pharmacistRole.Id))
            {
                _context.StaffRolePermissions.AddRange(
                    new StaffRolePermission { StaffRoleId = pharmacistRole.Id, Module = "Prescriptions", CanView = true, CanUpdate = true },
                    new StaffRolePermission { StaffRoleId = pharmacistRole.Id, Module = "Pharmacy", CanView = true, CanCreate = true, CanUpdate = true },
                    new StaffRolePermission { StaffRoleId = pharmacistRole.Id, Module = "Patients", CanView = true }
                );
            }

            // Receptionist permissions
            var receptionistRole = await _context.StaffRoles.FirstOrDefaultAsync(sr => sr.RoleName == "Receptionist");
            if (receptionistRole != null && !await _context.StaffRolePermissions.AnyAsync(p => p.StaffRoleId == receptionistRole.Id))
            {
                _context.StaffRolePermissions.AddRange(
                    new StaffRolePermission { StaffRoleId = receptionistRole.Id, Module = "Patients", CanView = true, CanCreate = true, CanUpdate = true },
                    new StaffRolePermission { StaffRoleId = receptionistRole.Id, Module = "Appointments", CanView = true, CanCreate = true, CanUpdate = true },
                    new StaffRolePermission { StaffRoleId = receptionistRole.Id, Module = "Billing", CanView = true }
                );
            }

            // Nurse permissions
            var nurseRole = await _context.StaffRoles.FirstOrDefaultAsync(sr => sr.RoleName == "Nurse");
            if (nurseRole != null && !await _context.StaffRolePermissions.AnyAsync(p => p.StaffRoleId == nurseRole.Id))
            {
                _context.StaffRolePermissions.AddRange(
                    new StaffRolePermission { StaffRoleId = nurseRole.Id, Module = "Patients", CanView = true },
                    new StaffRolePermission { StaffRoleId = nurseRole.Id, Module = "VitalSigns", CanView = true, CanCreate = true, CanUpdate = true },
                    new StaffRolePermission { StaffRoleId = nurseRole.Id, Module = "Appointments", CanView = true }
                );
            }

            // Lab Technician permissions
            var labTechRole = await _context.StaffRoles.FirstOrDefaultAsync(sr => sr.RoleName == "Lab Technician");
            if (labTechRole != null && !await _context.StaffRolePermissions.AnyAsync(p => p.StaffRoleId == labTechRole.Id))
            {
                _context.StaffRolePermissions.AddRange(
                    new StaffRolePermission { StaffRoleId = labTechRole.Id, Module = "Lab", CanView = true, CanCreate = true, CanUpdate = true },
                    new StaffRolePermission { StaffRoleId = labTechRole.Id, Module = "Patients", CanView = true }
                );
            }

            await _context.SaveChangesAsync();
        }
    }
}