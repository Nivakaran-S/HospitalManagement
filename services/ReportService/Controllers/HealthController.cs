using Microsoft.AspNetCore.Mvc;


namespace ReportService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "ReportService",
            timestamp = DateTime.UtcNow
        });
    }
}