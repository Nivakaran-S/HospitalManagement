using Microsoft.AspNetCore.Mvc;


namespace StaffService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "StaffService",
            timestamp = DateTime.UtcNow
        });
    }
}