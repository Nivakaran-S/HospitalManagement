using Microsoft.AspNetCore.Mvc;


namespace AppointmentService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new 
        { 
            status = "Healthy",
            service = "AppointmentService",  
            timestamp = DateTime.UtcNow 
        });
    }
}