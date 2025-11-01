using Microsoft.AspNetCore.Mvc;


namespace PatientService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "PatientService",
            timestamp = DateTime.UtcNow
        });
    }
}