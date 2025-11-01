using Microsoft.AspNetCore.Mvc;


namespace PaymentService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "PaymentService",
            timestamp = DateTime.UtcNow
        });
    }
}