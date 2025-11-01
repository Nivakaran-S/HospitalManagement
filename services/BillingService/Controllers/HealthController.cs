using Microsoft.AspNetCore.Mvc;


namespace BillingService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "BillingService",
            timestamp = DateTime.UtcNow
        });
    }
}