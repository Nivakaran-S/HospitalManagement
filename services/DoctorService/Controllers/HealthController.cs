using Microsoft.AspNetCore.Mvc;


namespace DoctorService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "DoctorService",
            timestamp = DateTime.UtcNow
        });
    }
}