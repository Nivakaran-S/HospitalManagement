using Microsoft.AspNetCore.Mvc;


namespace PrescriptionService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "PrescriptionService",
            timestamp = DateTime.UtcNow
        });
    }
}