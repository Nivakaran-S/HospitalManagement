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
    public class SystemSettingsController : ControllerBase
    {
        private readonly AdminContext _context;
        private readonly ILogger<SystemSettingsController> _logger;

        public SystemSettingsController(AdminContext context, ILogger<SystemSettingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemSettings>>> GetSettings()
        {
            var settings = await _context.SystemSettings
                .OrderBy(s => s.SettingKey)
                .ToListAsync();
            
            return Ok(settings);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{key}")]
        public async Task<ActionResult<SystemSettings>> GetSetting(string key)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == key);

            if (setting == null) return NotFound();
            return setting;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<SystemSettings>> PostSetting(SystemSettings setting)
        {
            if (await _context.SystemSettings.AnyAsync(s => s.SettingKey == setting.SettingKey))
                return BadRequest("Setting key already exists");

            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
            setting.UpdatedBy = userName;
            setting.UpdatedAt = DateTime.UtcNow;

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSetting), 
                new { key = setting.SettingKey }, setting);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSetting(int id, SystemSettings setting)
        {
            if (id != setting.Id) return BadRequest();

            var dbSetting = await _context.SystemSettings.FindAsync(id);
            if (dbSetting == null) return NotFound();

            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";

            dbSetting.SettingValue = setting.SettingValue;
            dbSetting.Description = setting.Description;
            dbSetting.UpdatedBy = userName;
            dbSetting.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbSetting).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            var setting = await _context.SystemSettings.FindAsync(id);
            if (setting == null) return NotFound();

            _context.SystemSettings.Remove(setting);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}