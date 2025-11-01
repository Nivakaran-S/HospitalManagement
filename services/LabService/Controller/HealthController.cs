using Microsoft.AspNetCore.Mvc;


namespace LabService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "LabService",
            timestamp = DateTime.UtcNow
        });
    }
}