using Microsoft.AspNetCore.Mvc;


namespace NotificationService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "NotificationService",
            timestamp = DateTime.UtcNow
        });
    }
}