using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationContext _context;
        private readonly INotificationSender _sender;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            NotificationContext context,
            INotificationSender sender,
            ILogger<NotificationController> logger)
        {
            _context = context;
            _sender = sender;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("my-notifications")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetMyNotifications(
            [FromQuery] bool? unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // In real scenario, get user ID from token
            int userId = 1; // Placeholder

            var query = _context.Notifications
                .Where(n => n.RecipientId == userId)
                .AsQueryable();

            if (unreadOnly == true)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync();
            var notifications = await query
                .OrderByDescending(n => n.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(notifications);
        }

        [Authorize]
        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await LogNotificationAction(id, "Read", "Notification marked as read");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpPost("send")]
        public async Task<ActionResult<Notification>> SendNotification(SendNotificationRequest request)
        {
            var notification = new Notification
            {
                RecipientId = request.RecipientId,
                RecipientType = request.RecipientType,
                NotificationType = request.NotificationType,
                Channel = request.Channel,
                Subject = request.Subject,
                Message = request.Message,
                Priority = request.Priority,
                ScheduledFor = request.ScheduledFor,
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType,
                Status = request.ScheduledFor.HasValue ? "Pending" : "Sent",
                SentAt = request.ScheduledFor ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send immediately if not scheduled
            if (!request.ScheduledFor.HasValue)
            {
                await _sender.SendAsync(notification);
                await LogNotificationAction(notification.Id, "Sent", $"Sent via {request.Channel}");
            }

            return CreatedAtAction(nameof(GetMyNotifications), new { id = notification.Id }, notification);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("send-bulk")]
        public async Task<ActionResult<object>> SendBulkNotifications(BulkNotificationRequest request)
        {
            var notifications = request.RecipientIds.Select(recipientId => new Notification
            {
                RecipientId = recipientId,
                RecipientType = request.RecipientType,
                NotificationType = request.NotificationType,
                Channel = request.Channel,
                Subject = request.Subject,
                Message = request.Message,
                Status = "Sent",
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // Send notifications asynchronously
            _ = Task.Run(async () =>
            {
                foreach (var notification in notifications)
                {
                    try
                    {
                        await _sender.SendAsync(notification);
                        await LogNotificationAction(notification.Id, "Sent", "Bulk notification sent");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send notification {notification.Id}");
                    }
                }
            });

            return Ok(new { Count = notifications.Count, Message = "Notifications queued for sending" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications(
            [FromQuery] string? status,
            [FromQuery] string? notificationType)
        {
            var query = _context.Notifications.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(n => n.Status == status);

            if (!string.IsNullOrEmpty(notificationType))
                query = query.Where(n => n.NotificationType == notificationType);

            var notifications = await query
                .OrderByDescending(n => n.SentAt)
                .Take(100)
                .ToListAsync();

            return Ok(notifications);
        }

        [Authorize]
        [HttpGet("preferences")]
        public async Task<ActionResult<NotificationPreference>> GetPreferences()
        {
            int userId = 1; // Get from token
            string userType = "Patient"; // Get from token

            var preferences = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId && np.UserType == userType);

            if (preferences == null)
            {
                preferences = new NotificationPreference
                {
                    UserId = userId,
                    UserType = userType
                };
                _context.NotificationPreferences.Add(preferences);
                await _context.SaveChangesAsync();
            }

            return Ok(preferences);
        }

        [Authorize]
        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences(NotificationPreference preferences)
        {
            int userId = 1; // Get from token

            var dbPreferences = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId);

            if (dbPreferences == null) return NotFound();

            dbPreferences.EmailEnabled = preferences.EmailEnabled;
            dbPreferences.SmsEnabled = preferences.SmsEnabled;
            dbPreferences.InAppEnabled = preferences.InAppEnabled;
            dbPreferences.PushEnabled = preferences.PushEnabled;
            dbPreferences.AppointmentReminders = preferences.AppointmentReminders;
            dbPreferences.PrescriptionUpdates = preferences.PrescriptionUpdates;
            dbPreferences.LabResults = preferences.LabResults;
            dbPreferences.BillingAlerts = preferences.BillingAlerts;
            dbPreferences.GeneralAnnouncements = preferences.GeneralAnnouncements;
            dbPreferences.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<NotificationTemplate>>> GetTemplates()
        {
            var templates = await _context.NotificationTemplates
                .Where(nt => nt.IsActive)
                .OrderBy(nt => nt.TemplateName)
                .ToListAsync();

            return Ok(templates);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("templates")]
        public async Task<ActionResult<NotificationTemplate>> CreateTemplate(NotificationTemplate template)
        {
            template.CreatedAt = DateTime.UtcNow;
            _context.NotificationTemplates.Add(template);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplates), new { id = template.Id }, template);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetStatistics([FromQuery] DateTime? fromDate)
        {
            var query = _context.Notifications.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(n => n.SentAt >= fromDate.Value);

            var stats = new
            {
                TotalSent = await query.CountAsync(n => n.Status == "Sent" || n.Status == "Delivered"),
                TotalFailed = await query.CountAsync(n => n.Status == "Failed"),
                TotalPending = await query.CountAsync(n => n.Status == "Pending"),
                ReadRate = await query.Where(n => n.Status == "Sent").AnyAsync()
                    ? (double)await query.CountAsync(n => n.IsRead) / await query.CountAsync(n => n.Status == "Sent") * 100
                    : 0,
                ByType = await query
                    .GroupBy(n => n.NotificationType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync(),
                ByChannel = await query
                    .GroupBy(n => n.Channel)
                    .Select(g => new { Channel = g.Key, Count = g.Count() })
                    .ToListAsync()
            };

            return Ok(stats);
        }

        private async Task LogNotificationAction(int notificationId, string action, string details)
        {
            var log = new NotificationLog
            {
                NotificationId = notificationId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.NotificationLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}