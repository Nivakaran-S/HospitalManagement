using Microsoft.AspNetCore.Mvc;


namespace PharmacyService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "PharmacyService",
            timestamp = DateTime.UtcNow
        });
    }
}